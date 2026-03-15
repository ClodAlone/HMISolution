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
using System.Diagnostics;

#if !WinRT
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
// using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Controls.Grid.Automation.Peers;
namespace Syncfusion.Windows.Controls.Grid

#else

using Syncfusion.WinRT.ComponentModel;
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using System.Reflection;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Input;
using Windows.System;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public partial class GridControlBase : VirtualizingCellsControl, IDisposable
    {
        #region Fields
        Brush highlightBrush;
        Brush highlightBorder;
        //DrawingVisual dvSelectBackground = new NoHitTestDrawingVisual();
        //DrawingVisual dvSelectBorder = new NoHitTestDrawingVisual();
        //DrawingVisual dvCurrentCellBorder = new NoHitTestDrawingVisual();
        Panel selectedCellsFrame = new VisualContainer("SelectedCellsFrame");
        Panel currentCellFrame = new VisualContainer("CurrentCellFrame");
        Panel selectBorderFrame = new VisualContainer("SelectBorder");
        Panel dragBorderFrame = new VisualContainer("DragBorderFrame");
        Panel CellCommentFrame = new VisualContainer("CellCommentFrame");
        Panel hiddenBorderFrame = new VisualContainer("HiddenBorderFrame");
        GridModel model = null;
        GridCellRendererCollection cellRenderers = null;
        GridCurrentCell currentCell;
        GridControlRenderStyles renderStyles;
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
        public static readonly DependencyProperty IgnoreChangedEventProperty = DependencyProperty.RegisterAttached(
            "IgnoreChangedEvent", typeof(bool?), typeof(GridControlBase), new PropertyMetadata(null));

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
#if !WinRT
        /// <summary>
        /// Sets the <see cref="IgnoreChangeProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetIgnoreChangedEvent(DependencyObject dpo, bool value)
        {
            dpo.SetValue(IgnoreChangedEventProperty, value);
        }
#endif
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

        public Brush HighlightBrush
        {
            get { return highlightBrush; }
            set { highlightBrush = value; }
        }
        public Brush HighlightBorder
        {
            get { return highlightBorder; }
            set { highlightBorder = value; }
        }

        double currentCellBorderWeight = 1.0f;

        public double CurrentCellBorderWeight
        {
            get { return currentCellBorderWeight; }
            set { currentCellBorderWeight = value; }
        }
#if !WinRT
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var point = e.GetPosition(this);
            if (this.model.GraphicModel.GraphicCells != null)
            {
                foreach (var span in this.Model.GraphicModel.GraphicCells)
                {
                    var style = this.Model.GraphicModel[span.CellSpanIndex];
                    if (style.GraphicCellControl != null)
                    {
                        var rect = VisualContainer.GetRenderBounds(style.GraphicCellControl);
                        if (rect.Contains(point))
                        {
                            this.Model.GraphicModel.SelectionController.OnMouseDown(e, span);
                            e.Handled = true;
                            return;
                        }
                    }
                }
            }
            base.OnMouseLeftButtonDown(e);
            if (Model != null)
            {
                Model.ActiveGridView = this;
            }
        }
#else 
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (Model.ActiveGridView != null)
                Model.ActiveGridView = this;
            if (!e.Handled && base.Focus(Windows.UI.Xaml.FocusState.Programmatic))
                e.Handled = true;
        }
#endif

        #region Ctor
        public GridControlBase()
        {
            renderStyles = new GridControlRenderStyles(this);
            //Focusable = true;
            this.MouseControllerDispatcher.Add(new GridResizeColumnsMouseController(this));
            this.MouseControllerDispatcher.Add(new GridResizeRowsMouseController(this));
            this.MouseControllerDispatcher.Add(new GridSelectCellsMouseController(this));
            ////MouseControllerDispatcher.Add(new GridClickCellsMouseController(this));

            //ForegroundFrame.Children.Add(dvSelectBackground);
            //ForegroundFrame.Children.Add(dvSelectBorder);
            //ForegroundFrame.Children.Add(dvCurrentCellBorder);
            //this.ForegroundFrame.Children.Add(this.selectedCellsFrame);
            
            this.BackgroundFrame.Children.Add(this.currentCellFrame);
            this.BackgroundFrame.Children.Add(this.selectedCellsFrame);
            //CellCommentFrame is added to InnerFrame since it is came above the header while scrolling.
            this.InnerFrame.Children.Add(this.CellCommentFrame);

            // this.ForegroundFrame.Children.Add(this.currentCellFrame);
            this.ForegroundFrame.Children.Add(this.selectBorderFrame);
            this.ForegroundFrame.Children.Add(this.hiddenBorderFrame);
            this.ForegroundFrame.Children.Add(this.dragBorderFrame);
            

            Color c = Colors.Blue;// SystemColors.HighlightColor;
            highlightBrush = new SolidColorBrush(Color.FromArgb(96, c.R, c.G, c.B));
            highlightBorder = new SolidColorBrush(Colors.Black);
            //HighlightBrush = new LinearGradientBrush(Color.FromArgb(192, c.R, c.G, c.B), Colors.OrangeRed, 45.0);
            //HighlightBrush = SystemColors.HighlightBrush;

            currentCell = new GridCurrentCell(this);

            // CoveredCellsProvider, CellSpanBackgroundsProvider, 
            // RowHeightsProvider and ColumnWidthsProvider are set
            // in Model property setter below.

            GridControlBase.SetCellsControl(this, this); // TODO: Debug MoveCurrentHelper with nested grids.
            //GridControlBase.SetWantsMouseInput(this, true);
            this.Loaded += new RoutedEventHandler(GridControlBase_Loaded);
        }
#if !WinRT
        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new GridControlAutomationPeer(this);
        }
#endif
        void GridControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.CurrentCell != null && this.CurrentCell.HasCurrentCell)
                this.CurrentCell.ScrollInView();
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
                        model.RemoveView(this);
                        model.VolatileCellStyles.BaseStylesMap.Dispose();
                        model.VolatileCellStyles.visibleRowIndexes.Remove(this);
                        model.VolatileCellStyles.visibleColumnIndexes.Remove(this);
                        if (cellRenderers != null)
                        {
                            cellRenderers.Dispose();
                            cellRenderers = null;
                        }
#if !WinRT
                        if (model.GraphicModel != null)
                        {
                            model.GraphicModel.Dispose();
                            model.GraphicModel = null;
                        }
#endif
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
#if !WinRT
                this.Model.UpdateAutoSizer(true, true);
#endif
            }
            else if (e.PropertyName == "MaxLength")
            {
#if !WinRT
                this.Model.Sizer.ApplySizes();
#endif
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
            Model = null;
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

        internal void UnwireModelEvents()
        {
            this.Model.BaseStylesMapChanged -= Model_BaseStylesMapChanged;
            //this.Model.CellModelsChanged -= Model_CellModelsChanged;
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
        }

        internal void WireModelEvents()
        {
            this.Model.BaseStylesMapChanged += Model_BaseStylesMapChanged;
            //this.Model.CellModelsChanged += Model_CellModelsChanged;
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
            }

            this.RenderCurrentCellBorder();

            this.OnSelectionChanged(e);
        }

        /// <summary>
        /// Invalidate cell so that PrepareRenderCell will get called again but not QueryCellStyle.
        /// </summary>
        /// <param name="range"></param>
        public void InvalidateRenderCell(GridRangeInfo range)
        {
            CellSpanInfoBase span = range.ToCellSpan(Model);
            RenderStyles.Clear(span);
            base.InvalidateCell(span);
        }

        public event GridSelectionChangedEventHandler SelectionChanged;
        protected virtual void OnSelectionChanged(GridSelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        #endregion
        #region SaveCellFormattedText

        void Model_SaveCellFormattedText(object sender, GridCellTextEventArgs e)
        {
            this.OnSaveCellFormattedText(e);
        }
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

        public event GridRangeInsertedEventHandler ColumnsInserted;
        protected virtual void OnColumnsInserted(GridRangeInsertedEventArgs e)
        {
            if (ColumnsInserted != null)
                ColumnsInserted(this, e);
        }

        #endregion
        #region CellModelsChanged

        //void Model_CellModelsChanged(object sender, CollectionChangeEventArgs e)
        //{
        //    this.OnCellModelsChanged(e);
        //}

        //public event CollectionChangeEventHandler CellModelsChanged;
        //protected virtual void OnCellModelsChanged(CollectionChangeEventArgs e)
        //{
        //    if (CellModelsChanged != null)
        //        CellModelsChanged(this, e);
        //}

        #endregion
        #region BaseStylesMapChanged

        void Model_BaseStylesMapChanged(object sender, EventArgs e)
        {
            this.OnBaseStylesMapChanged(e);
        }

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
            GridModel model = Model; // possibly creates Model on demand and assign linesizes.
            return base.MeasureOverride(constraint);
        }

        void modelCommitCellInfo(object sender, GridCommitCellInfoEventArgs e)
        {
            //if (e.Style.Store.IsValueModified(GridStyleInfoStore.BackgroundProperty))
            //    this.InvalidateCellBackground(e.Cell);
            //if (e.Style.Store.IsValueModified(GridStyleInfoStore.BordersProperty))
            //    this.InvalidateCellBorder(e.Cell);
        }

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
                {
                    Model = new GridModel();
                }
                return (GridOverlappingCellInfoCollection)OverlappingCellsProvider;
            }
        }

        public GridCellSpanBackgroundInfoCollection CellSpanBackgrounds
        {
            get
            {
                if (CellSpanBackgroundsProvider == null)
                    Model = new GridModel();

                return (GridCellSpanBackgroundInfoCollection)CellSpanBackgroundsProvider;
            }
        }

        public IEditableLineSizeHost RowHeights
        {
            get
            {
                if (RowHeightsProvider == null || RowHeightsProvider is EmptyLineSizeHost)
                    Model = new GridModel();

                return (IEditableLineSizeHost)RowHeightsProvider;
            }
        }

        public IEditableLineSizeHost ColumnWidths
        {
            get
            {
                if (ColumnWidthsProvider == null || ColumnWidthsProvider is EmptyLineSizeHost)
                    Model = new GridModel();

                return (IEditableLineSizeHost)ColumnWidthsProvider;
            }
        }

        public int FrozenRows
        {
            get
            {
                return RowHeights.HeaderLineCount;
            }
            set
            {
                RowHeights.HeaderLineCount = value;
            }
        }

        public int FrozenColumns
        {
            get
            {
                return ColumnWidths.HeaderLineCount;
            }
            set
            {
                ColumnWidths.HeaderLineCount = value;
            }
        }

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
        //protected override void OnRender(DrawingContext dc)
        //{
        //    RenderCurrentCellBorder();
        //    base.OnRender(dc);
        //}
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
            if (CurrentCell != null)
            {
                if (CurrentCell.IsEditing)
                {
                    CurrentCell.CreateCurrentCellUIElements();
                }
                this.CellCommentFrame.Children.Clear();
                this.hiddenBorderFrame.Children.Clear();
                base.OnArrangeContent(arrangeSize);
                this.RenderCurrentCellBorder();
                this.RenderHiddenCellBorder();
#if !WinRT
                if (this.Model.GraphicModel != null && this.Model.GraphicModel.GraphicCells.Count > 0)
                {
                    this.Model.GraphicModel.ArrangeGraphicCells();
                }
#endif
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets the cell background for a cell from the <see cref="IRenderCellInfo"/> cell style.
        /// </summary>
        /// <param name="ci">The cell style.</param>
        /// <returns></returns>
#if !WinRT
        protected override Brush GetCellBackground(IRenderCellInfo ci, bool combineBackgrounds)
        {
            GridRenderStyleInfo style = (GridRenderStyleInfo)ci;
            // If background stored in CellInfo is not a brush and instead a description of a brush (e.g. GridBrushInfo)
            // this method is a good place to convert that to a brush and cache it for reuse in other cells.
            if (combineBackgrounds)
                return style.ModelStyle.Background;

            return style.Background;
        }
#else
        protected override Brush GetCellBackground(IRenderCellInfo ci)
        {
            GridRenderStyleInfo style = (GridRenderStyleInfo)ci;
            return style.Background;
        }
#endif

#endif

        public virtual GridRangeInfo ExpandSelectedCellsRange(GridRangeInfo selectedCells)
        {
            return selectedCells.ExpandRange(Model.HeaderRows, Model.HeaderColumns, Model.RowCount, Model.ColumnCount);
        }

        protected virtual void RenderSelectedCells()
        {
            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.AlphaBlend) != 0 ) //|| (Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
            {
                GridRangeInfoList savedRanges = new GridRangeInfoList();
                this.selectedCellsFrame.Children.Clear();
                Rect gridRect = new Rect(0, 0, RenderSize.Width, RenderSize.Height);
                for (int n = 0; n < Model.SelectedRanges.Count; n++)
                {
                    GridRangeInfo selectedCells = Model.SelectedRanges[n];
                    //Rectangle shape = new Rectangle();
                    var shape = new Path() { Stretch = Stretch.Fill, IsHitTestVisible = false };
                    this.selectedCellsFrame.Children.Add(shape);
                    if (!selectedCells.IsEmpty)
                    {
                        selectedCells = this.ExpandSelectedCellsRange(selectedCells);

                        // Check for overlapping ranges, avoid them being drawn darker.
                        bool drawCellsIndividually = (Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.OverlapWithDarkenedAlphaBlend) == 0
                            && savedRanges.AnyRangeIntersects(selectedCells);

                        DoubleSpan[] yPos = ScrollRows.RangeToRegionPoints(selectedCells.Top, selectedCells.Bottom, true);
                        DoubleSpan[] xPos = ScrollColumns.RangeToRegionPoints(selectedCells.Left, selectedCells.Right, true);

                        GridRangeInfo currentCellRange = Model.Options.ShowCurrentCell ? CurrentCell.RangeInfo : GridRangeInfo.Empty;
                        //GridRangeInfo currentCellRange = CurrentCell.RangeInfo ;
                        CoveredCellInfo cc = CoveredCells.GetCellSpan(CurrentCell.RowIndex, CurrentCell.ColumnIndex);
                        if (cc != null)
                            currentCellRange = GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right);

                        DoubleSpan[] yCurrentCellPos = ScrollRows.RangeToRegionPoints(currentCellRange.Top, currentCellRange.Bottom, true);
                        DoubleSpan[] xCurrentCellPos = ScrollColumns.RangeToRegionPoints(currentCellRange.Left, currentCellRange.Right, true);

                        for (int rowRegion = 1; rowRegion <= 1; rowRegion++)
                        {
                            if (yPos[rowRegion].IsEmpty)
                                continue;

                            for (int columnRegion = 1; columnRegion <= 1; columnRegion++)
                            {
                                if (xPos[columnRegion].IsEmpty)
                                    continue;

                                Rect clipRect = GetClipRect((ScrollAxisRegion)rowRegion, (ScrollAxisRegion)columnRegion);
                                Rect clipRectHeaderRow = GetClipRect((ScrollAxisRegion)rowRegion - 1, (ScrollAxisRegion)columnRegion);
                                Rect clipRectHeaderColumn = GetClipRect((ScrollAxisRegion)rowRegion, (ScrollAxisRegion)columnRegion - 1);
                                Rect r = new Rect(xPos[columnRegion].Start, yPos[rowRegion].Start, xPos[columnRegion].Length, yPos[rowRegion].Length);

                                #region FrozenHeader

                                if (this.Model.HeaderRows < this.Model.FrozenRows
                                    || this.Model.HeaderColumns < this.Model.FrozenColumns)
                                {
                                    var frozenShape = new Path() { Stretch = Stretch.Fill, IsHitTestVisible = false };
                                    this.selectedCellsFrame.Children.Add(frozenShape);
                                    var frozenHeaderRowSelectionRect = Rect.Empty;
                                    if (this.Model.HeaderRows < this.Model.FrozenRows)
                                    {
                                        frozenHeaderRowSelectionRect = this.RangeToClippedVisibleRect(GridRangeInfo.Cells(this.Model.HeaderRows, this.Model.HeaderColumns, this.Model.FrozenRows - 1, this.Model.ColumnCount - 1));
                                    }

                                    var frozenHeaderColumnSelectionRect = Rect.Empty;

                                    if (this.Model.HeaderColumns < this.Model.FrozenColumns)
                                    {
                                        frozenHeaderColumnSelectionRect = this.RangeToClippedVisibleRect(GridRangeInfo.Cells(this.Model.HeaderRows, this.Model.HeaderColumns, this.Model.RowCount - 1, this.Model.FrozenColumns - 1));
                                    }

                                    var selectedCellsRect = this.RangeToClippedVisibleRect(selectedCells);
                                    frozenHeaderRowSelectionRect.Intersect(selectedCellsRect);
                                    frozenHeaderColumnSelectionRect.Intersect(selectedCellsRect);
                                    var geometryGroup1 = new GeometryGroup();

                                    if (n == Model.SelectedRanges.Count - 1)
                                    {
                                        var cRect = Rect.Empty;
                                        if (this.CurrentCell.RowIndex >= 0 && this.CurrentCell.ColumnIndex >= 0)
                                        {
                                            cRect = this.RangeToClippedVisibleRect(currentCellRange);
                                        }

                                        if (this.Model.HeaderColumns >= 1 && this.Model.HeaderRows >= 1)
                                        {
                                            var headerCellsR = this.RangeToClippedVisibleRect(GridRangeInfo.Cells(0, 0, this.Model.HeaderRows - 1, this.Model.ColumnCount - 1));
                                            var headerCellsC = this.RangeToClippedVisibleRect(GridRangeInfo.Cells(0, 0, this.Model.RowCount - 1, this.Model.HeaderColumns - 1));

                                            var frozenR = this.RangeToClippedVisibleRect(GridRangeInfo.Cells(this.Model.HeaderRows, this.Model.HeaderColumns, this.Model.FrozenRows - 1, this.Model.ColumnCount));
                                            var frozenC = this.RangeToClippedVisibleRect(GridRangeInfo.Cells(this.Model.HeaderRows, this.Model.HeaderColumns, this.Model.RowCount, this.Model.FrozenColumns - 1));

                                            headerCellsR.Intersect(cRect);
                                            headerCellsC.Intersect(cRect);

                                            bool allowCRect = (headerCellsC.Width > 0 && headerCellsC.Height > 0 || headerCellsR.Width > 0 && headerCellsR.Height > 0) || (cRect.Left > frozenC.Right && cRect.Top > frozenR.Bottom);

                                            if (!cRect.IsEmpty && !allowCRect)
                                            {
                                                geometryGroup1.Children.Add(new RectangleGeometry() { Rect = cRect });
                                            }
                                        }
                                    }

                                    var intersection = frozenHeaderColumnSelectionRect;
                                    intersection.Intersect(frozenHeaderRowSelectionRect);
                                    if (!intersection.IsEmpty)
                                    {
                                        var intersectionPath = new Path() { Stretch = Stretch.Fill, IsHitTestVisible = false };
                                        var geometryGroup2 = new GeometryGroup();
                                        geometryGroup2.Children.Add(new RectangleGeometry() { Rect = frozenHeaderRowSelectionRect });
                                        this.selectedCellsFrame.Children.Add(intersectionPath);
                                        geometryGroup2.Children.Add(new RectangleGeometry() { Rect = intersection });
                                        geometryGroup1.Children.Add(new RectangleGeometry() { Rect = intersection });
                                        DrawSelectedCellsRectangle(intersectionPath, geometryGroup2, frozenHeaderRowSelectionRect);
                                    }

                                    if (!frozenHeaderRowSelectionRect.IsEmpty)
                                    {                                      
                                        //This code added Because to avoid the Dropdown row selection draw over the current cell selection.
                                        if (!(this.CurrentCell.CellRowColumnIndex.ColumnIndex < -1 && CurrentCell.CellRowColumnIndex.RowIndex < -1))
                                        {
                                            Rect cellRect = RangeToClippedVisibleRect(this.CurrentCell.RangeInfo);
                                            if (!cellRect.IsEmpty)
                                                geometryGroup1.Children.Add(new RectangleGeometry() { Rect = cellRect });
                                        }
                                        //Till This

                                        geometryGroup1.Children.Add(new RectangleGeometry() { Rect = frozenHeaderRowSelectionRect });
                                        if (!this.Model.Options.ExcelLikeSelectionFrame)
                                            DrawSelectedCellsRectangle(frozenShape, geometryGroup1, frozenHeaderRowSelectionRect);
                                    }

                                    if (!frozenHeaderColumnSelectionRect.IsEmpty)
                                    {
                                        geometryGroup1.Children.Add(new RectangleGeometry() { Rect = frozenHeaderColumnSelectionRect });
                                        DrawSelectedCellsRectangle(frozenShape, geometryGroup1, frozenHeaderColumnSelectionRect);
                                    }
                                }
                                #endregion

                                //System.Diagnostics.Debug.WriteLine(clipRect.ToString() + " " + clipRectHeaderRow.ToString() + " " + r.ToString());
                                if (!clipRect.IntersectsWith(r))
                                    continue;

                                if (clipRectHeaderRow.IntersectsWith(r))
                                {
                                    r.Height = r.Height + r.Top - clipRectHeaderRow.Bottom;
                                    r.Y = clipRectHeaderRow.Bottom;
                                }

                                if (clipRectHeaderColumn.IntersectsWith(r))
                                {
                                    r.Width = r.Width + r.Left - clipRectHeaderColumn.Right;
                                    r.X = clipRectHeaderColumn.Right;
                                }

                                // Background
                                Rect currentCellRect = Rect.Empty;
                                var geometryGroup = new GeometryGroup();
                                bool addCurrentCellGeometry = true;
                                if (!xCurrentCellPos[columnRegion].IsEmpty && !yCurrentCellPos[rowRegion].IsEmpty)
                                {
                                    // Exclude current cell
                                    currentCellRect = new Rect(xCurrentCellPos[columnRegion].Start, yCurrentCellPos[rowRegion].Start, xCurrentCellPos[columnRegion].Length, yCurrentCellPos[rowRegion].Length);

                                    var totalHeight = currentCellRect.Height + currentCellRect.Top;
                                    if (clipRectHeaderRow.IntersectsWith(currentCellRect) && currentCellRect.Top > 0 && totalHeight > clipRectHeaderRow.Bottom)
                                    {
                                        currentCellRect.Height = totalHeight - clipRectHeaderRow.Bottom;
                                        currentCellRect.Y = clipRectHeaderRow.Bottom;
                                    }

                                    var totalWidth = currentCellRect.Width + currentCellRect.Left;
                                    if (clipRectHeaderColumn.IntersectsWith(currentCellRect) /* && currentCellRect.Left > 0 */ && totalWidth > clipRectHeaderColumn.Right) // Highlight selection background issue SD    . currentCellRect.Left has negative value to reduce the width of the cell while scrolling Also this should pass this condition to render the Highlight selection background frame.
                                    {
                                        currentCellRect.Width = totalWidth - clipRectHeaderColumn.Right;
                                        currentCellRect.X = clipRectHeaderColumn.Right;
                                    }

                                    if (totalWidth <= clipRectHeaderColumn.Right || totalHeight <= clipRectHeaderRow.Bottom)
                                    {
                                        addCurrentCellGeometry = false;
                                    }

                                    if (!(this.CurrentCell.CellRowColumnIndex.ColumnIndex < -1 && CurrentCell.CellRowColumnIndex.RowIndex < -1))
                                    {
                                        if (n == Model.SelectedRanges.Count - 1 && addCurrentCellGeometry)
                                        {                   
#if !WinRT
                                            var datagrid = this.FindParentElementOfType<GridDataControl>() as GridDataControl;
                                            if(datagrid == null)
                                            {
                                                geometryGroup.Children.Add(new RectangleGeometry() { Rect = currentCellRect });
                                            }
                                            else if (datagrid != null)
                                            {
                                                geometryGroup.Children.Add(new RectangleGeometry() { Rect = currentCellRect });
                                            }    
#endif
                                        }
                                    }
                                }

                                if (!drawCellsIndividually)
                                {                                    
                                    geometryGroup.Children.Add(new RectangleGeometry() { Rect = r });
                                    DrawSelectedCellsRectangle(shape, geometryGroup, r);
                                }
                                else
                                {
                                    Int32Span regionRows = ScrollRows.GetVisibleLinesRange(rowRegion);
                                    Int32Span regionColumns = ScrollColumns.GetVisibleLinesRange(columnRegion);
                                    GridRangeInfo regionRange = GridRangeInfo.Cells(regionRows.Start, regionColumns.Start, regionRows.End, regionColumns.End);

                                    GridRangeInfo range = selectedCells.IntersectRange(regionRange);
                                    if (range.IsEmpty)
                                        return;

                                    for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                                    {
                                        for (int columnIndex = range.Left; columnIndex <= range.Right; columnIndex++)
                                        {
                                            GridRangeInfo cell = GridRangeInfo.Cell(rowIndex, columnIndex);
                                            if (savedRanges.AnyRangeContains(cell))
                                            {
                                                var r3 = RangeToClippedVisibleRect(cell);
                                                geometryGroup.Children.Add(new RectangleGeometry() { Rect = r3 });
                                            }

                                            var r2 = RangeToClippedVisibleRect(cell);
                                            geometryGroup.Children.Add(new RectangleGeometry() { Rect = r2 });
                                        }
                                    }

                                    this.DrawSelectedCellsRectangle(shape, geometryGroup, r);
                                }
                            }
                        }

                        savedRanges.Add(selectedCells);
                    }
                }
            }

            this.RenderActiveRangeBorder();
        }

        protected virtual void DrawCommentCellsTriangle(Path backgroundRectangle, Geometry g, Rect r)
        {
#if !WinRT
            backgroundRectangle.Fill = Brushes.Red;
#else
            backgroundRectangle.Fill = new SolidColorBrush(Colors.Red);
#endif
            backgroundRectangle.Data = g;
            backgroundRectangle.Arrange(r);
        }

        protected virtual void DrawSelectedCellsRectangle(Path backgroundRectangle, Geometry g, Rect r)
        {
            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.AlphaBlend) != 0)
            {
                backgroundRectangle.Fill = Model.Options.HighlightSelectionAlphaBlend;
            }
            else if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
            {
                backgroundRectangle.Fill = Model.Options.HighlightSelectionBackground;
            }
            backgroundRectangle.Data = g;
            backgroundRectangle.Arrange(r);
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

        protected virtual void RenderActiveRangeBorder()
        {
            //DrawingContext dccBorder = dvSelectBorder.RenderOpen();
            if (this.model.Options.ExcelLikeSelectionFrame)
                this.selectBorderFrame.Children.Clear();

            this.dragBorderFrame.Children.Clear();
            if (Model.Options.ExcelLikeSelectionFrame
                && Model.SelectedRanges.Count > 0)
            {
                Rect gridRect = new Rect(0, 0, RenderSize.Width, RenderSize.Height);
                GridRangeInfo selectedCells = Model.SelectedRanges[Model.SelectedRanges.Count - 1];
                var shape = new Path() { IsHitTestVisible = false, Stretch = Stretch.Fill };
                this.selectBorderFrame.Children.Add(shape);
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

                            #region FrozenHeader

                            if (Model.SelectedRanges.Count > 0 && (this.Model.HeaderRows != 0 && this.Model.HeaderColumns != 0) &&
                                (this.Model.HeaderRows < this.Model.FrozenRows || this.Model.HeaderColumns < this.Model.FrozenColumns))
                            {
                                var cRect = Rect.Empty;
                                cRect = this.RangeToClippedVisibleRect(selectedCells);

                                if (!cRect.IsEmpty)
                                {
                                    r = cRect;
                                }
                                else
                                    continue;
                            }

                            #endregion

                            Rect selectedRect = this.RangeToClippedVisibleRect(selectedCells);
                            if (selectedRect.IsEmpty)
                            {
                                r.Intersect(clipRect);
                            }
                            else
                            {
                                r = selectedRect;
                            }
                         
                            //Cell Border Fix
                            if (r.Width >= 1 && r.Height >= 1)
                            {
                                r = new Rect(r.Left + .5, r.Top + .5, r.Width - 1, r.Height - 1);
                            }

                            // Border
                            var geometryGroup = new GeometryGroup();
                            //geometryGroup.Children.Add(new RectangleGeometry() { Rect = clipRect });

                            geometryGroup.Children.Add(new RectangleGeometry() { Rect = r });
                            double w = Model.Options.HighlightSelectionBorderWidth;
                            double w2 = w + 6;
                            shape.Stroke = Model.Options.HighlightSelectionBorder;
                            shape.StrokeThickness = w;
                            shape.StrokeStartLineCap = PenLineCap.Round;
                            shape.StrokeEndLineCap = PenLineCap.Round;

                            Rect markerRect = Rect.Empty;
                            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ExcelLikeSelectionMarker) != 0)
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
                                
                                geometryGroup.Children.Add(new RectangleGeometry() { Rect = markerRect });
                            }

                            if (!markerRect.IsEmpty)
                            {
                                markerRect.Inflate(-2, -2);
                                shape.Data = new RectangleGeometry() { Rect = markerRect };
                             //   shape.Fill = Model.Options.HighlightSelectionBorder;
                                shape.Arrange(r);
                            }
                            else
                            {
                                shape.Data = geometryGroup; //new RectangleGeometry() { Rect = r };
                                shape.Arrange(r);
                            }
                        }
                    }
                }
            }
            //dccBorder.Close();
        }

        protected virtual void RenderHiddenCellBorder()
        {
            if (!Model.Options.AllowExcelLikeResizing)
            {
                return;
            }

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
                            if (visibleRow.IsHeader && rowIndex == 0)
                            {
                                cellRect.X = visibleColumn.Origin;
                                cellRect.Width = visibleColumn.Size;

                                IRenderCellInfo renderCellInfo = GetRenderCellInfo(rowIndex, columnIndex);
                                var rca = new RenderCellArgs(this, visibleRow, visibleColumn, cellRect, renderCellInfo);
                                OnRenderHiddenColBorder(rca, preceding);
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
                            int rowIndex = visibleRow.LineIndex;
                            var colIndex = visibleCol.LineIndex;
                            if (visibleCol.IsHeader && colIndex == 0)
                            {
                                var cellRect = new Rect(0, visibleCol.Origin, ScrollColumns.ViewSize, visibleCol.Size);
                                cellRect.X = visibleRow.Origin;
                                cellRect.Width = visibleRow.Size;

                                IRenderCellInfo renderCellInfo = GetRenderCellInfo(rowIndex, colIndex);
                                var rca = new RenderCellArgs(this, visibleRow, visibleCol, cellRect, renderCellInfo);
                                OnRenderHiddenRowBorder(rca, preceding);
                            }
                        }
                    }
                }
            }

        }


        #region RenderHiddenColBorder

        protected virtual void OnRenderHiddenColBorder(RenderCellArgs rca, bool preceding)
        {
            if (rca.ColumnIndex > 0)
            {
                var coveredRange = this.Model.CoveredCells.GetCoveredCell(rca.RowIndex, rca.ColumnIndex);
                var isCovered = coveredRange != null;

                int rc, rh;
                var hidden = this.ColumnWidths.GetHidden(rca.ColumnIndex, out rc);
                var drawHidden = this.ColumnWidths.GetHidden(rca.ColumnIndex + (preceding ? -1 : 1), out rh);

                var startPoint = preceding ? new Point(rca.CellClipRect.Left, rca.CellClipRect.Top) : new Point(rca.CellClipRect.Right, rca.CellClipRect.Top);
                var endPoint = preceding ? new Point(rca.CellClipRect.Left, rca.CellClipRect.Bottom) : new Point(rca.CellClipRect.Right, rca.CellClipRect.Bottom);

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
                    this.DrawHiddenBorder(startPoint, endPoint, rca.CellClipRect);
                    //dc.DrawLine(new Pen(this.model.HiddenBorderBrush, this.Model.HiddenBorderThikness), startPoint, endPoint);
                }
            }
        }

        #endregion


        #region RenderHiddenRowBorder

        protected virtual void OnRenderHiddenRowBorder(RenderCellArgs rca, bool preceding)
        {
            if (rca.RowIndex > 0)
            {
                var coveredRange = this.Model.CoveredCells.GetCoveredCell(rca.RowIndex, rca.ColumnIndex);
                var isCovered = coveredRange != null;

                int rc, rh;
                var hidden = this.RowHeights.GetHidden(rca.RowIndex, out rc);
                var drawHidden = this.RowHeights.GetHidden(rca.RowIndex + (preceding ? -1 : 1), out rh);

                var startPoint = preceding ? new Point(rca.CellClipRect.Left, rca.CellClipRect.Top) : new Point(rca.CellClipRect.Left, rca.CellClipRect.Bottom);
                var endPoint = preceding ? new Point(rca.CellClipRect.Right, rca.CellClipRect.Top) : new Point(rca.CellClipRect.Right, rca.CellClipRect.Bottom);

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
                    this.DrawHiddenBorder(startPoint, endPoint, rca.CellClipRect);
                    //dc.DrawLine(new Pen(this.model.HiddenBorderBrush, this.Model.HiddenBorderThikness), );
                }
            }
        }

        private void DrawHiddenBorder(Point startPoint, Point endPoint, Rect rect)
        {
            var shape = new Path() { IsHitTestVisible = false, Stretch = Stretch.Fill };
            this.hiddenBorderFrame.Children.Add(shape);
            shape.Stroke = Model.Options.HiddenBorderBrush;
            shape.StrokeThickness = Model.Options.HiddenBorderThickness;
            shape.StrokeStartLineCap = PenLineCap.Round;
            shape.StrokeEndLineCap = PenLineCap.Round;
            shape.Data = new LineGeometry() { StartPoint = startPoint, EndPoint = endPoint };
            //shape.Arrange(rect);
        }

        #endregion

        /// <summary>
        /// Render current cell border into the dvCurrentCellBorder DrawingVisual of the ScrollControl.ForegroundFrame. This will not trigger any InvalidateVisual or InvalidateArrange calls.
        /// </summary>
        protected internal virtual void RenderCurrentCellBorder()
        {
            //DrawingContext dccBorder = dvCurrentCellBorder.RenderOpen();

            if (!this.model.Options.ExcelLikeSelectionFrame)
                this.selectBorderFrame.Children.Clear();

            this.currentCellFrame.Children.Clear();
            if (ShouldRenderCurrentCellBorder() && CurrentCell.HasCurrentCell && Model.Options.ShowCurrentCell && !Model.Options.ExcelLikeSelectionFrame)
            {
                var shape = new Path() { IsHitTestVisible = false, Stretch = Stretch.Fill };
                this.currentCellFrame.Children.Add(shape);
                GridRangeInfo currentCellRange = CurrentCell.RangeInfo;
                CoveredCellInfo cc = CoveredCells.GetCellSpan(CurrentCell.RowIndex, CurrentCell.ColumnIndex);
                if (cc != null)
                    currentCellRange = GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right);
                ////This condition is to specify the cell range to provide the current cell border.
                ////As the current cell would become multiple cell range while floating.
                var containsflag = floatcellran.ContainsKey(CurrentCell.CellRowColumnIndex);
                
                if (containsflag && CurrentCell.IsEditing && Model.Options.EnableFloatCell)
                    currentCellRange = GridRangeInfo.Cells(CurrentCell.RowIndex, CurrentCell.ColumnIndex, CurrentCell.RowIndex, floatcellran[CurrentCell.CellRowColumnIndex]);
                DoubleSpan[] yCurrentCellPos = ScrollRows.RangeToRegionPoints(currentCellRange.Top, currentCellRange.Bottom, true);
                DoubleSpan[] xCurrentCellPos = ScrollColumns.RangeToRegionPoints(currentCellRange.Left, currentCellRange.Right, true);

                for (int rowRegion = 1; rowRegion <= 1; rowRegion++)
                {
                    if (yCurrentCellPos[rowRegion].IsEmpty)
                        continue;

                    for (int columnRegion = 1; columnRegion <= 1; columnRegion++)
                    {
                        if (xCurrentCellPos[columnRegion].IsEmpty)
                            continue;

                        Rect clipRect = GetClipRect((ScrollAxisRegion)rowRegion, (ScrollAxisRegion)columnRegion);
                        Rect r = new Rect(xCurrentCellPos[columnRegion].Start, yCurrentCellPos[rowRegion].Start, xCurrentCellPos[columnRegion].Length, yCurrentCellPos[rowRegion].Length);
                        Rect clipRectHeaderRow = GetClipRect((ScrollAxisRegion)rowRegion - 1, (ScrollAxisRegion)columnRegion);
                        Rect clipRectHeaderColumn = GetClipRect((ScrollAxisRegion)rowRegion, (ScrollAxisRegion)columnRegion - 1);

                        var totalHeight = r.Height + r.Top;
                        if (clipRectHeaderRow.IntersectsWith(r) && r.Top > 0 && totalHeight > clipRectHeaderRow.Bottom)
                        {
                            r.Height = Math.Floor(totalHeight - clipRectHeaderRow.Bottom);
                            r.Y = Math.Floor(clipRectHeaderRow.Bottom);
                        }

                        var totalWidth = r.Width + r.Left;                        
                        if (clipRectHeaderColumn.IntersectsWith(r) && (r.Left > 0 || totalWidth > clipRectHeaderColumn.Right))
                        {
                            if (clipRectHeaderColumn.Right <= totalWidth)
                                r.Width = Math.Floor(totalWidth - clipRectHeaderColumn.Right);
                            else
                                r.Width = 0;
                            r.X = Math.Floor(clipRectHeaderColumn.Right);
                        }

                        #region FrozenHeader
                        bool isFrozen = false;
                        if (Model.SelectedRanges.Count > 0 &&
                            (this.Model.HeaderRows < this.Model.FrozenRows || this.Model.HeaderColumns < this.Model.FrozenColumns))
                        {
                            GridRangeInfo selectedCells = Model.SelectedRanges[Model.SelectedRanges.Count - 1];

                            /// Filter bar and add new rows are frozen row, which should show the selection even if there is no header rows, hence 'or' is used
                            /// and based on condition changed code in the the if statement is chagned.
                            if (this.Model.HeaderColumns >= 1 || this.Model.HeaderRows >= 1)
                            {
                                Rect frozenHeaderRowSelectionRect = Rect.Empty;
                                Rect frozenHeaderColumnSelectionRect = Rect.Empty;

                                if (this.model.FrozenRows > 0)
                                    frozenHeaderRowSelectionRect = this.RangeToClippedVisibleRect(GridRangeInfo.Cells(this.Model.HeaderRows, this.Model.HeaderColumns, this.Model.FrozenRows - 1, this.Model.ColumnCount - 1));

                                if (this.model.FrozenColumns > 0)
                                    frozenHeaderColumnSelectionRect = this.RangeToClippedVisibleRect(GridRangeInfo.Cells(this.Model.HeaderRows, this.Model.HeaderColumns, this.Model.RowCount - 1, this.Model.FrozenColumns - 1));

                                var selectedCellsRect = this.RangeToClippedVisibleRect(selectedCells);
                                frozenHeaderRowSelectionRect.Intersect(selectedCellsRect);
                                frozenHeaderColumnSelectionRect.Intersect(selectedCellsRect);

                                var cRect = Rect.Empty;
                                if (this.CurrentCell.RowIndex >= 0 && this.CurrentCell.ColumnIndex >= 0)
                                {
                                    cRect = this.RangeToClippedVisibleRect(currentCellRange);
                                }

                                if (!cRect.IsEmpty)
                                {
                                    if (!frozenHeaderRowSelectionRect.IsEmpty && cRect.Top >= frozenHeaderRowSelectionRect.Top && cRect.Bottom <= frozenHeaderRowSelectionRect.Bottom && cRect.Left >= frozenHeaderRowSelectionRect.Left)
                                    {
                                        r = cRect;
                                        isFrozen = true;
                                    }

                                    if (!frozenHeaderColumnSelectionRect.IsEmpty && cRect.Left >= frozenHeaderColumnSelectionRect.Left && cRect.Right <= frozenHeaderColumnSelectionRect.Right && cRect.Top >= frozenHeaderColumnSelectionRect.Top)
                                    {
                                        r = cRect;
                                        isFrozen = true;
                                    }
                                }
                            }
                        }

                        #endregion

                        if ((totalWidth > clipRectHeaderColumn.Right && totalHeight > clipRectHeaderRow.Bottom)
                           || isFrozen)
                        {
                            //Cell Border Fix
                            if (r.Width >= 1 && r.Height >= 1)
                            {
                                r = new Rect(r.Left + .5, r.Top + .5, r.Width - 1, r.Height - 1);
                            }
                            //New path for the border is created and added to selectborderframe.
                            Path borderShape = new Path() { IsHitTestVisible = false, Stretch = Stretch.Fill };
                            this.selectBorderFrame.Children.Add(borderShape);

                            //Previous shape's border is commented.
                            //shape.Stroke = Model.Options.CurrentCellBorder;
                            //shape.StrokeThickness = Model.Options.CurrentCellBorderWidth;
                            shape.StrokeStartLineCap = PenLineCap.Round;
                            shape.StrokeEndLineCap = PenLineCap.Round;
                            shape.Data = new RectangleGeometry() { Rect = r };
#if !WinRT
                            if (!(Model is GridDataTableModel) && !(Model is GridTreeModel) && this.Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(CurrentCell.RowIndex, CurrentCell.ColumnIndex)))
                            {
                                if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
                                {
                                    shape.Fill = Model.Options.HighlightSelectionBackground;
                                }
                            }
#endif

                            shape.Arrange(r);

                            //Data for path is given below and this was added to the selectnackgroundframes.
                            borderShape.Data = new RectangleGeometry() { Rect = r };
                            borderShape.Stroke = model.Options.CurrentCellBorder;
                            borderShape.StrokeThickness = model.Options.CurrentCellBorderWidth;
                            borderShape.Fill = new SolidColorBrush(Colors.Transparent);

                            borderShape.Arrange(r);
                        }
                    }
                }
            }
            //dccBorder.Close();

            RenderSelectedCells();
        }

        //internal virtual Brush GetCurrentCellBackground()
        //{
        //    return Model.Options.HighlightSelectionBackground;
        //}

        protected virtual bool ShouldRenderCurrentCellBorder()
        {
            return true;
        }

        protected override void RenderCellBorders()
        {
            base.RenderCellBorders();
                VisibleLinesCollection visiblelinesInColumn = ScrollColumns.GetVisibleLines();
                VisibleLinesCollection visiblelinesInRow = ScrollRows.GetVisibleLines();
            if (Model.Options.ExcelLikeFreezePane)
            {
                int headerColumn = (ScrollColumns.HeaderLineCount - 1);
                int headerRow = (ScrollRows.HeaderLineCount - 1);
                CellBorderRange frozenBorder = new CellBorderRange(new Pen(new SolidColorBrush(Colors.Black), 1.00, BorderStyle.Standard, null), 1);
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
                    OnRenderBorderShape(r1, r1, CellBorderSide.Right, frozenBorder);
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
                    OnRenderBorderShape(r2, r2, CellBorderSide.Bottom, frozenBorder);
                }
            }
            ////Cell border to be disabled between the floating and flooding cells
            foreach (KeyValuePair<RowColumnIndex, int> val in floatcellran)
            {
                for (int i = val.Key.ColumnIndex; i < val.Value; i++)
                {
                    VisibleLineInfo row = ScrollRows.GetVisibleLineAtLineIndex(val.Key.RowIndex);
                    VisibleLineInfo col = ScrollColumns.GetVisibleLineAtLineIndex(i + 1);
                    if (row != null && col != null)
                    {
                        Brush color = this.Model[row.LineIndex, col.LineIndex].Background;
                        Rect r = GridUtil.FromLTRB(Math.Floor(col.ClippedOrigin), Math.Floor(row.ClippedOrigin), Math.Floor(col.ClippedOrigin), Math.Floor(row.ClippedCorner));
                        CellBorderRange floatborder = new CellBorderRange(new Pen(color, 1.00, BorderStyle.Standard, null), 1);
                        OnRenderBorderShape(r, r, CellBorderSide.Right, floatborder);
                    }
                }
            }
        }

        #endregion

        #region Resize Rows and Columns Controller Hooks

        public virtual void SetRowHeight(int rowIndex, double size)
        {
            if (!RowHeights.SupportsNestedLines || RowHeights.GetNestedLines(rowIndex) == null)
                RowHeights[rowIndex] = size;
            InvalidateVisual();
        }

        public virtual void SetColumnWidth(int columnIndex, double size)
        {
            if (!ColumnWidths.SupportsNestedLines || ColumnWidths.GetNestedLines(columnIndex) == null)
                ColumnWidths[columnIndex] = size;
            InvalidateVisual();
        }

        #endregion

        #region Virtualized GridRenderStyleInfo

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
#if !WinRT
            if (this is GridDataControlBaseImpl)
            {
                this.EnableRenderOptimization = ((GridDataControlBaseImpl)this).EnableRenderOptimization;
            }
#endif
            base.ArrangeCellUIElements(arrangeSize);

            RenderStyles.ConcludeArrange();
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            //if (ClearVisualsCacheWhenUnloaded)
            //{
            //    if (model != null)
            //    {
            //        model.VolatileCellStyles.visibleRowIndexes.Remove(this);
            //        model.VolatileCellStyles.visibleColumnIndexes.Remove(this);
            //    }

            //    RenderStyles.Clear();
            //}
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

        public GridRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex)
        {
            return RenderStyles.GetRenderStyleInfo(rowIndex, columnIndex);
        }

        public GridRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex)
        {
            return RenderStyles.GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        public GridRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex, bool createDisposableObject)
        {
            return RenderStyles.GetRenderStyleInfo(rowIndex, columnIndex, createDisposableObject);
        }

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
        /// <param name="cellRowColumnIndex"></param>
        public override void InvalidateCell(RowColumnIndex cellRowColumnIndex)
        {
            RenderStyles.Clear(cellRowColumnIndex);
            Model.VolatileCellStyles.Clear(cellRowColumnIndex);
            base.InvalidateCell(cellRowColumnIndex);
        }

        public override void InvalidateCell(CellSpanInfoBase span)
        {
            RenderStyles.Clear(span);
            Model.VolatileCellStyles.Clear(span);
            base.InvalidateCell(span);
        }

        public void InvalidateCell(GridRangeInfo gridRangeInfo)
        {
            CellSpanInfoBase span = gridRangeInfo.ToCellSpan(Model);
            InvalidateCell(span);
        }

        public override void UnloadArrangedCells()
        {
            RenderStyles.Clear();
            Model.VolatileCellStyles.Clear();
            base.UnloadArrangedCells();
        }

        #endregion

        #region PrepareRenderCell Event

        internal virtual void RaisePrepareRenderCell(GridPrepareRenderCellEventArgs e)
        {
            if ((Model.Options.DrawSelectionOptions & (GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor)) != 0)
            {
                if (e.Cell.RowIndex >= Model.HeaderRows && e.Cell.ColumnIndex >= Model.HeaderColumns)
                {
                    if (Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
                    {
#if  !WinRT
                        if (Model is GridDataTableModel)
                        {
                            var currentStyle = (e.Style as GridRenderStyleInfo).ModelStyle;
                            var tableStyleIdentity = (currentStyle as GridDataStyleInfo).CellIdentity;

                            if (tableStyleIdentity.TableCellType == GridDataTableCellType.RecordPlusMinusCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.EmptyCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.NestedTableEmptyCell)
                                return;
                        }
#endif

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
            if (PrepareRenderCell != null)
                PrepareRenderCell(this, e);
        }

        public event GridPrepareRenderCellEventHandler PrepareRenderCell;

        #endregion

        #region Insert and Remove Rows

        protected internal virtual void ModelInsertRows(int insertAtRowIndex, int count, GridViewMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridViewMoveCellsState.Empty;

            RenderStyles.InsertRows(insertAtRowIndex, count, moveCellsState.RenderStyles);
            ArrangedCellUIElements.InsertRows(insertAtRowIndex, count, moveCellsState.CellUIElements);
            CurrentCell.InsertRows(insertAtRowIndex, count, moveCellsState.CurrentCell);

            ScrollRows.MarkDirty();
            InvalidateVisual(true);
        }

        protected internal virtual void ModelRemoveRows(int removeAtRowIndex, int count, GridViewMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridViewMoveCellsState.Empty;

            RenderStyles.RemoveRows(removeAtRowIndex, count, moveCellsState.RenderStyles);
            ArrangedCellUIElements.RemoveRows(removeAtRowIndex, count, moveCellsState.CellUIElements);
            CurrentCell.RemoveRows(removeAtRowIndex, count, moveCellsState.CurrentCell);

            ScrollRows.MarkDirty();
            InvalidateVisual(true);
        }

        #endregion

        #region Insert and Remove Columns

        protected internal virtual void ModelInsertColumns(int insertAtColumnIndex, int count, GridViewMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridViewMoveCellsState.Empty;

            RenderStyles.InsertColumns(insertAtColumnIndex, count, moveCellsState.RenderStyles);
            ArrangedCellUIElements.InsertColumns(insertAtColumnIndex, count, moveCellsState.CellUIElements);
            CurrentCell.InsertColumns(insertAtColumnIndex, count, moveCellsState.CurrentCell);

            ScrollColumns.MarkDirty();
            InvalidateVisual(true);
        }

        protected internal virtual void ModelRemoveColumns(int removeAtColumnIndex, int count, GridViewMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridViewMoveCellsState.Empty;

            RenderStyles.RemoveColumns(removeAtColumnIndex, count, moveCellsState.RenderStyles);
            ArrangedCellUIElements.RemoveColumns(removeAtColumnIndex, count, moveCellsState.CellUIElements);
            CurrentCell.RemoveColumns(removeAtColumnIndex, count, moveCellsState.CurrentCell);

            ScrollColumns.MarkDirty();
            InvalidateVisual(true);
        }

        #endregion

        #region RangeToRect
        public Rect RangeToClippedVisibleRect(RowColumnIndex cellRowColumnIndex)
        {
            return RangeToClippedVisibleRect(cellRowColumnIndex, true);
        }

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

        /// <summary>
        /// Raises the <see cref="CancelMode"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCancelMode(EventArgs e)
        {
            if (CancelMode != null)
                CancelMode(this, e);
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
        /// 
#if !WinRT
        [
        Description("Occurs when the window receives a WM_CANCELMODE message."),
        Category("Behavior")
        ]
#endif
        public event EventHandler CancelMode;

        #endregion

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

        /// <summary>
        /// Gets or sets the OleDragDropEventsTarget. Redirects events defined in <see cref="IGridOleDragDropEventsTarget"/> to the specified object.
        /// Each event will first be called on <see cref="IGridOleDragDropEventsTarget"/> before the actual
        /// event handler in this control is called.
        /// </summary>
#if !WinRT
        [Browsable(false)]
#endif
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
                catch
                {
#if WPF
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
#endif

                    e.Cancel = true;
                }
            }
        }

        #endregion

        #endregion
        internal void RenderExcelLikeBorder(Rect r)
        {
            var coll = new DoubleCollection();
            coll.Add(4);
            coll.Add(4);
            this.dragBorderFrame.Children.Clear();
            var geometryGroup = new GeometryGroup();

            //Cell Border Fix
            if (r.Width >= 1 && r.Height >= 1)
            {
                r = new Rect(r.Left + .5, r.Top + .5, r.Width - 1, r.Height - 1);
            }

            geometryGroup.Children.Add(new RectangleGeometry() { Rect = r });
            var shape = new Path() { IsHitTestVisible = false, Stretch = Stretch.Fill };
            this.dragBorderFrame.Children.Add(shape);
            shape.Data = geometryGroup;
            System.Diagnostics.Debug.WriteLine(r.ToString());
            shape.Stroke = new SolidColorBrush(Colors.Black);
            shape.StrokeThickness = 2;
            shape.StrokeStartLineCap = PenLineCap.Round;
            shape.StrokeEndLineCap = PenLineCap.Round;
            shape.StrokeDashOffset = .2;
            shape.StrokeDashArray = coll;
            shape.Arrange(r);
        }

        internal virtual Rect RenderExcelRangeBorder(GridRangeInfo range)
        {
            if (Model.Options.ExcelLikeSelectionFrame)
            {
                Rect gridRect = new Rect(0, 0, RenderSize.Width, RenderSize.Height);
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

        #region CurrentCell Navigation

        internal GridCurrentCellMoveDelegateHandler externalMove;

        /// <summary>
        /// Used by GridSelectCellsMouseController.
        /// </summary>
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

        /// <summary>
        /// Occurs before the grid is about to be left because the user is at the top-left or bottom-right
        /// cell and about to tab out of the grid.
        /// This event is only raised if the <see cref="GridWrapCellBehavior.NextControlInForm"/>
        /// has been specified for <see cref="GridModelOptions.WrapCell"/>.
        /// </summary>
        /// <seealso cref="GridWrapCellNextControlInFormEventHandler"/>
        public event GridWrapCellNextControlInFormEventHandler WrapCellNextControlInForm;

        /// <summary>
        /// Raises the <see cref="WrapCellNextControlInForm"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridWrapCellNextControlInFormEventArgs" /> that contains the event data.</param>
        protected virtual void OnWrapCellNextControlInForm(GridWrapCellNextControlInFormEventArgs e)
        {
            if (WrapCellNextControlInForm != null)
                WrapCellNextControlInForm(this, e);
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

        public event GridMoveCurrentCellDirectionEventHandler MoveCurrentCellDirection;

        /// <summary>
        /// Raises the  <see cref="MoveCurrentCellDirection"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridMoveCurrentCellDirectionEventArgs" /> that contains the event data.</param>
        protected virtual void OnMoveCurrentCellDirection(GridMoveCurrentCellDirectionEventArgs e)
        {
            if (MoveCurrentCellDirection != null)
                MoveCurrentCellDirection(this, e);
        }

        internal void RaiseMoveCurrentCellDirection(GridMoveCurrentCellDirectionEventArgs e)
        {
            OnMoveCurrentCellDirection(e);
        }

        #endregion

        #region QueryNextCurrentCellPosition

        public event GridQueryNextCurrentCellPositionEventHandler QueryNextCurrentCellPosition;

        /// <summary>
        /// Raises the <see cref="QueryNextCurrentCellPosition"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryNextCurrentCellPositionEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryNextCurrentCellPosition(GridQueryNextCurrentCellPositionEventArgs e)
        {
            if (QueryNextCurrentCellPosition != null)
                QueryNextCurrentCellPosition(this, e);
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
            GridQueryNextCurrentCellPositionEventArgs e = new GridQueryNextCurrentCellPositionEventArgs(direction, rowIndex, colIndex, this);
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

        #region CurrentCell Key Input

#if SyncfusionFramework4_0
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
            if (!e.Handled)
            {
#if !WinRT
                if (this.Model.GraphicModel != null && this.Model.GraphicModel.CurrentCellRenderers != null)
                {
                    return;
                }
                else
#endif
                {
                    IGridCellRenderer renderer = CurrentCell.Renderer;
                    if (renderer != null && !_isControlKey)
                        renderer.RaiseGridPreviewTextInput(e);
                }
            }
        }
#endif
        bool _isControlKey = false;
#if !WinRT

        protected internal virtual bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            _isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
#else
        protected internal virtual bool ShouldGridTryToHandlePreviewKeyDown(KeyRoutedEventArgs e)
        {
            _isControlKey = (Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control))== CoreVirtualKeyStates.Down;
#endif
            IGridCellRenderer renderer = CurrentCell.Renderer;
            if (renderer != null)
                return renderer.ShouldGridTryToHandlePreviewKeyDown(e);

            return false;
        }

        //protected override void OnPreviewKeyDown(KeyEventArgs e)
        //{
        //    base.OnPreviewKeyDown(e);

        //    if (!e.Handled)
        //    {
        //        if (ShouldGridTryToHandlePreviewKeyDown(e))
        //        {
        //            MoveCurrentCellWithArrowKey(e);
        //        }
        //    }
        //}

#if WinRT
        protected override void OnKeyDown(KeyRoutedEventArgs e)
#else
        protected override void OnKeyDown(KeyEventArgs e)
#endif
        {
            base.OnKeyDown(e);

            if (!e.Handled)
            {
#if!WinRT
                graphicCellPreviewHandled = ShouldGraphicCellTryToHandlePreviewKeyDown(e);
                if (!graphicCellPreviewHandled)
#endif
                {
                    var cellKeyHandled = this.OnCurrentCellPreviewKeyDown(new GridCellKeyEventArgs(e));
                    if (!cellKeyHandled && ShouldGridTryToHandlePreviewKeyDown(e))
                    {
                        MoveCurrentCellWithArrowKey(e);
                    }
                }
            }
#if !WinRT
#if SyncfusionFramework4_0
            switch (e.Key)
            {
                case Key.C:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        if (this.Model.CutPaste.CanCopy())
                        {
                            this.Model.CutPaste.Copy();
                            e.Handled = true;
                        }

                    }
                    break;

                case Key.X:

                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        if (this.Model.CutPaste.CanCut())
                        {
                            this.Model.CutPaste.Cut();
                            e.Handled = true;
                        }

                    }
                    break;

                case Key.V:

                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        if (this.Model.CutPaste.CanPaste())
                        {
                            this.Model.CutPaste.Paste();
                            e.Handled = true;
                        }

                    }
                    break;

                case Key.Insert:

                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        if (this.Model.CutPaste.CanCopy())
                        {
                            this.Model.CutPaste.Copy();
                            e.Handled = true;
                        }
                    }
                    else if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (this.Model.CutPaste.CanPaste())
                        {
                            this.Model.CutPaste.Paste();
                            e.Handled = true;
                        }

                    }
                    break;

                case Key.Delete:

                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (this.Model.CutPaste.CanCut())
                        {
                            this.Model.CutPaste.Cut();
                            e.Handled = true;
                        }
                    }
                    break;

                case Key.Z:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        if (!this.CurrentCell.IsEditing && this.Model.ShouldRecordUndo)
                        {
                            this.Model.CommandStack.Undo();
                            e.Handled = true;
                        }
                    }
                    break;

                case Key.Y:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        if (!this.CurrentCell.IsEditing && this.Model.ShouldRecordUndo)
                        {
                            this.Model.CommandStack.Redo();
                            e.Handled = true;
                        }
                    }
                    break;
            }

#endif
#endif
        }
#if WinRT
        public virtual bool MoveCurrentCellWithArrowKey(KeyRoutedEventArgs e)
        {
            bool isControlKey = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control)== CoreVirtualKeyStates.Down;
            bool isShiftKey = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Shift)== CoreVirtualKeyStates.Down;
            
            RowColumnIndex rc = CurrentCell.CellRowColumnIndex;

            if (Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Menu)== CoreVirtualKeyStates.None)
            {
                bool isRightToLeft = false;// base.FlowDirection == FlowDirection.RightToLeft;
                switch (e.Key)
                {
                    case VirtualKey.Escape:
                        CurrentCell.CancelEdit();
                        ///The below code will Reset the cache when you call the CancelEdit. Otherwise cache will not be cleared.
#if!WinRT
                        if ((this is GridDataControlBaseImpl && ((GridDataControlBaseImpl)this) != null))
                            (((GridDataControlBaseImpl)this).Model as GridDataTableModel).CurrencyManager.ResetCache();
#endif
                        e.Handled = true;
                        break;

                    case VirtualKey.F2:
                        if (CurrentCell.IsEditing)
                        {
                            CurrentCell.EndEdit();
                            // RefreshCell(CurrentCell.CellRowColumnIndex);
                        }
                        else
                            CurrentCell.BeginEdit();
                        e.Handled = true;
                        break;

                    case VirtualKey.Enter:
                        CurrentCell.EndEdit();
                        CurrentCell.ScrollInView();
                        e.Handled = true;
                        break;

                    case VirtualKey.Tab:
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
                            {
                                CurrentCell.MoveRight();
                            }
                        }
                        else
                        {
                            if (isShiftKey)
                                CurrentCell.MoveRight();
                            else
                                CurrentCell.MoveLeft();
                        }
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case VirtualKey.PageUp:
                        if (isControlKey)
                            CurrentCell.PageLeft();
                        else
                            CurrentCell.PageUp();
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case VirtualKey.PageDown:
                        if (isControlKey)
                            CurrentCell.PageRight();
                        else
                            CurrentCell.PageDown();
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case VirtualKey.End:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToBottomRight();
                        else
                            CurrentCell.MoveToRightEnd();
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case VirtualKey.Home:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToTopLeft();
                        else
                            CurrentCell.MoveToLeftEnd();
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case VirtualKey.Left:
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
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case VirtualKey.Up:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToTop();
                        else
                            CurrentCell.MoveUp();
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case VirtualKey.Right:
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
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case VirtualKey.Down:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToBottom();
                        else 
                            CurrentCell.MoveDown();
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;
                    case VirtualKey.A:
                        if (isControlKey && !this.CurrentCell.IsEditing)
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
#else
        public virtual bool MoveCurrentCellWithArrowKey(KeyEventArgs e)
        {
            bool isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;

            
            RowColumnIndex rc = CurrentCell.CellRowColumnIndex;

            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.None)
            {
                bool isRightToLeft = false;// base.FlowDirection == FlowDirection.RightToLeft;
                switch (e.Key)
                {
                    case Key.Escape:
                        CurrentCell.CancelEdit();
                        ///The below code will Reset the cache when you call the CancelEdit. Otherwise cache will not be cleared.
                        if ((this is GridDataControlBaseImpl && ((GridDataControlBaseImpl)this) != null))
                            (((GridDataControlBaseImpl)this).Model as GridDataTableModel).CurrencyManager.ResetCache();
                        e.Handled = true;
                        break;

                    case Key.F2:
                        if (CurrentCell.IsEditing)
                        {
                            CurrentCell.EndEdit();
                            // RefreshCell(CurrentCell.CellRowColumnIndex);
                        }
                        else
                            CurrentCell.BeginEdit();
                        e.Handled = true;
                        break;

                    case Key.Enter:
                        CurrentCell.EndEdit();
                        CurrentCell.ScrollInView();
                        e.Handled = true;
                        break;

                    case Key.Tab:
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
                            {
                                CurrentCell.MoveRight();
                            }
                        }
                        else
                        {
                            if (isShiftKey)
                                CurrentCell.MoveRight();
                            else
                                CurrentCell.MoveLeft();
                        }
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.PageUp:
                        if (isControlKey)
                            CurrentCell.PageLeft();
                        else
                            CurrentCell.PageUp();
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.PageDown:
                        if (isControlKey)
                            CurrentCell.PageRight();
                        else
                            CurrentCell.PageDown();
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.End:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToBottomRight();
                        else
                            CurrentCell.MoveToRightEnd();
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.Home:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToTopLeft();
                        else
                            CurrentCell.MoveToLeftEnd();
                        currentCell.ScrollInView();
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
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.Up:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToTop();
                        else
                            CurrentCell.MoveUp();
                        currentCell.ScrollInView();
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
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.Down:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToBottom();
                        else 
                            CurrentCell.MoveDown();
                        currentCell.ScrollInView();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;
                    case Key.A:
                        if (isControlKey && !this.CurrentCell.IsEditing)
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
#endif


        internal bool graphicCellPreviewHandled;
        protected internal virtual bool ShouldGraphicCellTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
#if!WinRT
            if (this.Model.GraphicModel != null && this.Model.GraphicModel.CurrentCellRenderers != null)
            {
                return this.Model.GraphicModel.CurrentCellRenderers.ShouldTryToHandlePreviewKeyDown(e);
            }
#endif
            return false;
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
                // var moveNextRow = false; The variable is assigned but it is never used.
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
            bool isValidated = true;
            if (!readOnly)
                CurrentCell.MoveTo(targetRow, targetCol, options, out isValidated);
        }

        private bool IncrementRowColCount(ref int row, ref int col)
        {
            col += 1;
            if (col >= Model.ColumnCount)
            {
                row += 1;
                col = 0;
                if (row >= Model.RowCount)
                    return false;
            }
            return true;
        }

        #endregion

        #region CellCommentService
        ////This is to measure the text along with the styles and formats applied
        internal Size MeasureText(string text, GridStyleInfo style)
        {
            var font = style.ReadOnlyFont;
            var textBlock = new TextBlock()
            {
                FontSize = font.FontSize,
                FontFamily = font.FontFamily,
                Text = text,
                TextWrapping = style.TextWrapping,
                TextTrimming = style.TextTrimming,
                FontStretch = font.FontStretch,
                FontWeight = font.FontWeight,
                FontStyle = font.FontStyle,
                HorizontalAlignment = style.HorizontalAlignment,
                Margin = style.TextMargins.ToThickness(),
                Padding = style.BorderMargins.ToThickness(),
#if !WinRT
                TextDecorations = font.TextDecorations,
#endif
                VerticalAlignment = style.VerticalAlignment
            };

            var parentBorder = new Border() { Child = textBlock };
            //textBlock.MaxWidth = clientSize.Width;
            var totalHeight = this.Model.RowHeights.TotalExtent;
            parentBorder.MaxHeight = totalHeight;
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            parentBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            parentBorder.Arrange(new Rect(0, 0, textBlock.ActualWidth, textBlock.ActualHeight));
            // very odd, when query is for row heights, then just do arrange/measure on the parent border and then return the textblock size
            var reservedTextSize = new Size(textBlock.ActualWidth, textBlock.ActualHeight);
            return reservedTextSize;
        }

        ////The dictionary collection is to maintain the collection of the floating cells and its range of floating
        ////The row column index is the cell to be maintained, and the int is the column range till expanded
        internal Dictionary<RowColumnIndex, int> floatcellran = new Dictionary<RowColumnIndex, int>();
        protected override void OnArrangeCell(ArrangeCellArgs aca)
        {
            var shape = new Path() { Stretch = Stretch.Fill, IsHitTestVisible = false };
            var style = RenderStyles.GetRenderStyleInfo(aca.RowIndex, aca.ColumnIndex);
            ////Starts the calculation and specification of the cell width with regards to the text and the cell element width
            if ((style.FloatCellsMode == GridFloatCellsMode.OnDemandCalculation || this.Model.Options.FloatCellMode == GridFloatCellsMode.OnDemandCalculation) && !style.HasTextWrapping)
            {
                int extendedcolumnindex = aca.ColumnIndex;
                Rect floatcellrect = aca.CellRect;
                string text = "";
                ////Retriving the text for getting the size
                bool IsEditing = false;
                if (aca.CellRowColumnIndex == CurrentCell.CellRowColumnIndex && CurrentCell.IsEditing)
                {
                    if (!(style.EnableFloatCell || Model.Options.EnableFloatCell))
                        IsEditing = true;
                    if (currentCell.Renderer.HasControlText)
                        text = CurrentCell.Renderer.ControlText;
                }
                else if (style.CellType == "FormulaCell")
                    text = style.FormattedText;
                else
                    text = style.HasFormat ? style.FormattedText : style.Text;

                if (text != string.Empty && style.RowIndex >= Model.HeaderRows && style.ColumnIndex >= Model.HeaderColumns && !IsEditing)
                {
                    if (floatcellran.ContainsKey(aca.CellRowColumnIndex))
                        floatcellran[aca.CellRowColumnIndex] = extendedcolumnindex;
                    ////measures the size of the text
                    var twidth = MeasureText(text, style).Width + 2;
                    if (style.HasTextMargins)
                    {
                        twidth += style.TextMargins.Left;
                    }
                    var colcount = this.Model.ColumnCount;
                    ////condition loop until the cell range size is less than the text size
                    while (twidth > floatcellrect.Width && colcount > extendedcolumnindex + 1)
                    {
                        if (style.HorizontalAlignment == HorizontalAlignment.Right)
                        {
                            if (extendedcolumnindex - 1 > 0)
                                extendedcolumnindex--;
                            else
                                break;
                        }
                        else
                            extendedcolumnindex++;
                        string internalcelltype = style.CellType;
                        var nextcellstyle = RenderStyles.GetRenderStyleInfo(aca.RowIndex, extendedcolumnindex);
                        string nextcelltype = nextcellstyle.CellType;
                        ////To avoid extending beyond the frozen column
                        if (style.HorizontalAlignment == HorizontalAlignment.Right)
                        {
                            if (this.Model.FrozenColumns == extendedcolumnindex - 1)
                            {
                                style.VerticalAlignment = VerticalAlignment.Top;
                                break;
                            }
                        }
                        else
                        {
                            if (this.Model.FrozenColumns == extendedcolumnindex)
                            {
                                style.VerticalAlignment = VerticalAlignment.Top;
                                break;
                            }
                        }
                        ////To avoid floating the covered cell through below process, which may conflict on the internal process.
                        if (GetVisibleCoveredCell(aca.VisibleRow, aca.VisibleColumn) != null)
                            break;
                        ////To avoid floating the image cell through below process, which may conflict on the internal process.
                        if (GetVisibleOverlappingCell(aca.VisibleRow, aca.VisibleColumn) != null)
                            break;
                        ////Conditions to check if the floating behaviors are enabled
                        if (nextcellstyle.Text != ""
                            || (CurrentCell.ColumnIndex == extendedcolumnindex && CurrentCell.RowIndex == aca.RowIndex
                            && CurrentCell.Renderer.IsEditable && !nextcellstyle.ReadOnly && CurrentCell.IsEditing)
                            || !nextcellstyle.FloodCell || !this.Model.Options.FloodCell
                            || this.Model.ColumnCount < extendedcolumnindex)
                            break;
                        ////This condition is to check if the cell and the next extending cell is a text area and not any other button contain cell type
                        if ((internalcelltype == "TextBox" || internalcelltype == "TextBlock" || internalcelltype == "FormulaCell"
                             || internalcelltype == "CurrencyEdit" || internalcelltype == "MaskEdit" || internalcelltype == "RichText"
                            || internalcelltype == "PercentEdit" || internalcelltype == "DoubleEdit" || internalcelltype == "Hyperlink")
                            && (nextcelltype == "TextBox" || nextcelltype == "TextBlock" || nextcelltype == "FormulaCell"
                             || nextcelltype == "CurrencyEdit" || nextcelltype == "MaskEdit" || nextcelltype == "RichText"
                            || nextcelltype == "PercentEdit" || nextcelltype == "DoubleEdit" || nextcelltype == "Hyperlink"))
                        {
                            double frozenrectx = floatcellrect.X;
                            double frozenrecty = floatcellrect.Y;
                            ////The calculated rectangle area beyond the exact single cell area
                            floatcellrect = RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cells(aca.RowIndex, aca.ColumnIndex, aca.RowIndex, extendedcolumnindex), false, false);
                            ////if the x axis goes below the column header area then have to place it in the same position
                            ////this is to avaoid calculation on frozen column, in which only the cell goes below the header and not the column
                            if (this.Model.FrozenColumns > aca.ColumnIndex)
                            {
                                floatcellrect.X = frozenrectx;
                            }
                            if (this.Model.FrozenRows > aca.RowIndex)
                            {
                                floatcellrect.Y = frozenrecty;
                            }
                        }
                        else
                            break;
                        ////adding the index and range to the collection
                        if (floatcellran.ContainsKey(aca.CellRowColumnIndex))
                            floatcellran[aca.CellRowColumnIndex] = extendedcolumnindex;
                        else
                            floatcellran.Add(aca.CellRowColumnIndex, extendedcolumnindex);
                    }
                }
                ////providing cell range with the size
                aca.CellRect = floatcellrect;
            }
            else if (floatcellran.ContainsKey(aca.CellRowColumnIndex))
            {
                floatcellran.Remove(aca.CellRowColumnIndex);
            }
            base.OnArrangeCell(aca);

            if (style.HasImageIndex && style.HasImageList)
            {
                var cellRect = aca.CellRect;
                var image = new Image();
                image = style.ImageList[style.ImageIndex];

                if (style.ImageWidth.GridUnitType == GridUnitType.Pixel)
                {
                    if (style.ImageWidth.Value != 0.2)
                        image.Width = style.ImageWidth.Value;
                    if (style.ImageHeight.Value != 1.0)
                        image.Height = style.ImageHeight.Value;
                }
                else
                {
                    if (image.Width.ToString() == "NaN")
                        style.ImageList[style.ImageIndex].Width = cellRect.Width;
                    if (image.Height.ToString() == "NaN")
                        style.ImageList[style.ImageIndex].Height = cellRect.Height;
                }

                var imgRect = Rect.Empty;
                var imageContentAlignment = style.ReadOnlyImageContentAlignment;
                var imageContentStretch = style.ReadOnlyImageContentStretch;
                var width = imageContentStretch == ImageContentStretch.Fill ? style.GetImageWidth() * cellRect.Width : style.GetImageWidth() * this.Model.ColumnWidths.GetDefaultLineSize();
                var height = imageContentStretch == ImageContentStretch.Fill ? style.GetImageHeight() * cellRect.Height : style.GetImageHeight() * this.Model.RowHeights.GetDefaultLineSize();


                if (imageContentAlignment == ImageContentAlignment.Left)
                {
                    imgRect = new Rect(cellRect.X, cellRect.Y, width, height);
                }
                else if (imageContentAlignment == ImageContentAlignment.Right)
                {
                    if (style.ImageWidth.GridUnitType == GridUnitType.Pixel)
                    {
                        imgRect = new Rect(cellRect.X + (cellRect.Width - (width + style.ImageWidth.Value)) + style.ImageMargins.Left, cellRect.Y + style.ImageMargins.Top, style.ImageMargins.Right, height + style.ImageMargins.Bottom);
                    }
                    else
                    {
                        imgRect = new Rect(cellRect.X + (cellRect.Width - (image.Width)), cellRect.Y, style.ImageMargins.Right, height + style.ImageMargins.Bottom);
                    }
                }
#if!WinRT
                if (imgRect != Rect.Empty)
                {

                        imgRect = style.AdjustImageMargins(imgRect);
                        var grid = new System.Windows.Controls.Grid();
                        image.Arrange(imgRect);
                        if (image.Parent != null)
                        {
                            var parent = (image.Parent) as System.Windows.Controls.Grid;
                            parent.Children.Remove(image);
                            grid.Children.Add(image);
                        }
                        else
                        {
                            grid.Children.Add(image);
                        }
                        grid.Margin = new Thickness(imgRect.X, imgRect.Y, 0, 0);
                        this.CellCommentFrame.Children.Add(grid);
                }
#endif
            }
#if !WinRT
            if (style.HasErrorInfo && style.ErrorInfo.HasErrorMessage)
            {
                if (this.CurrentCell.IsEditing && this.CurrentCell.RowIndex == style.RowIndex && this.CurrentCell.ColumnIndex== style.ColumnIndex && !this.Model.Options.ShowErrorIconOnEditing)
                    return;
                var cellRect = aca.CellRect;
                var contentAlignment = style.ErrorInfo.ReadOnlyErrorContentAlignment;
                Rect imgRect = Rect.Empty;
#if !SILVERLIGHT
                var width = style.ErrorInfo.GetImageWidth() * cellRect.Width;
                var height = style.ErrorInfo.GetImageHeight() * cellRect.Height;
#else
                var width = 20d;
                var height = 20d;
#endif
                if (contentAlignment == ImageContentAlignment.Left)
                {
                    //imgRect = new Rect(cellRect.X, cellRect.Y, GridTooltipService.ErrorImageBounds, cellRect.Height);
                    imgRect = new Rect(cellRect.X, cellRect.Y, width, height);
                }
                else if (contentAlignment == ImageContentAlignment.Right)
                {
                    //imgRect = new Rect(cellRect.X + (cellRect.Width - (GridTooltipService.ErrorImageBounds + 2)), cellRect.Y, GridTooltipService.ErrorImageBounds, cellRect.Height);
                    imgRect = new Rect(cellRect.X + (cellRect.Width - width), cellRect.Y, width, height);
                }
                if (imgRect != Rect.Empty)
                {
                    imgRect = style.ErrorInfo.AdjustImageMargins(imgRect);
                    switch (style.ErrorInfo.ReadOnlyErrorType)
                    {
                        case ErrorType.ErrorMessage:
                            var errorCanvas = GetErrorMessageCanvas();
                            errorCanvas.Margin = new Thickness(imgRect.X, imgRect.Y, 0, 0);
                            errorCanvas.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                            this.CellCommentFrame.Children.Add(errorCanvas);
                            break;
                        case ErrorType.Information:
                            var canvas = GetErrorInformationCavas();
                            canvas.Margin = new Thickness(imgRect.X, imgRect.Y, 0, 0);
                            canvas.Arrange(imgRect);
                            canvas.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                            this.CellCommentFrame.Children.Add(canvas);
                            break;
                        case ErrorType.Custom:
                            if (style.ErrorInfo.HasCustomImage)
                            {
                                var grid = new System.Windows.Controls.Grid();
                                Image img = new Image();
                                img.Source = style.ErrorInfo.CustomImage;
                                grid.Children.Add(img);
                                grid.Margin = new Thickness(imgRect.X, imgRect.Y, 0, 0);
                                this.CellCommentFrame.Children.Add(grid);
                            }
                            break;
                    }
                }
            }


            if (style.Comment != null || style.CommentTemplateKey != null)
            {
                switch (style.CommentAlignment)
                {
                    case CommentAlignment.BottomLeft:
                        {
                            this.CellCommentFrame.Children.Add(shape);
                            Rect r = aca.CellRect;
                            r = new Rect(r.Left, r.Top + r.Height - 10, 10, 10);
                            var geometryGroup = new GeometryGroup();
                            PathGeometry pg = new PathGeometry();
                            PathFigure pfRight = new PathFigure();
                            pfRight.StartPoint = new Point(0, 0);
                            pfRight.IsClosed = true;
                            pfRight.IsFilled = true;
                            PolyLineSegment pls = new PolyLineSegment();
                            pls.Points.Add(new Point(0, 0));
                            pls.Points.Add(new Point(2, 2));
                            pls.Points.Add(new Point(0, 2));
                            pfRight.Segments.Add(pls);
                            pg.Figures.Add(pfRight);
                            geometryGroup.Children.Add(pg);
                            DrawCommentCellsTriangle(shape, geometryGroup, r);
                        }
                        break;
                    case CommentAlignment.TopLeft:
                        {
                            this.CellCommentFrame.Children.Add(shape);
                            Rect r = aca.CellRect;
                            r.Width = 10;
                            r.Height = 10;
                            var geometryGroup = new GeometryGroup();
                            PathGeometry pg = new PathGeometry();
                            PathFigure pfRight = new PathFigure();
                            pfRight.StartPoint = new Point(0, 0);
                            pfRight.IsClosed = true;
                            pfRight.IsFilled = true;
                            PolyLineSegment pls = new PolyLineSegment();
                            pls.Points.Add(new Point(0, 0));
                            pls.Points.Add(new Point(2, 0));
                            pls.Points.Add(new Point(0, 2));
                            pfRight.Segments.Add(pls);
                            pg.Figures.Add(pfRight);
                            geometryGroup.Children.Add(pg);
                            DrawCommentCellsTriangle(shape, geometryGroup, r);
                        }
                        break;

                    case CommentAlignment.TopRight:
                        {
                            this.CellCommentFrame.Children.Add(shape);
                            Rect r = aca.CellRect;
                            int x = Convert.ToInt16(r.X);
                            int y = Convert.ToInt16(r.Y);
                            int width = Convert.ToInt16(r.Left + r.Width);
                            r = new Rect(r.Left + r.Width - 10, r.Top, 10, 10);
                            var geometryGroup = new GeometryGroup();
                            PathGeometry pg = new PathGeometry();
                            PathFigure pfRight = new PathFigure();
                            pfRight.StartPoint = new Point(width - 2, y);
                            pfRight.IsClosed = true;
                            pfRight.IsFilled = true;
                            PolyLineSegment pls = new PolyLineSegment();
                            pls.Points.Add(new Point(width, y));
                            pls.Points.Add(new Point(width, y + 2));
                            pfRight.Segments.Add(pls);
                            pg.Figures.Add(pfRight);
                            geometryGroup.Children.Add(pg);
                            DrawCommentCellsTriangle(shape, geometryGroup, r);
                            break;
                        }

                    case CommentAlignment.BottomRight:
                        {
                            {
                                this.CellCommentFrame.Children.Add(shape);
                                Rect r = aca.CellRect;
                                int x = Convert.ToInt16(r.X);
                                int y = Convert.ToInt16(r.Y);
                                int width = Convert.ToInt16(r.Left + r.Width);
                                r = new Rect(r.Left + r.Width - 10, r.Top + r.Height - 10, 10, 10);
                                var geometryGroup = new GeometryGroup();
                                PathGeometry pg = new PathGeometry();
                                PathFigure pfRight = new PathFigure();
                                pfRight.StartPoint = new Point(width - 2, y + 2);
                                pfRight.IsClosed = true;
                                pfRight.IsFilled = true;
                                PolyLineSegment pls = new PolyLineSegment();
                                pls.Points.Add(new Point(width, y));
                                pls.Points.Add(new Point(width, y + 2));
                                pfRight.Segments.Add(pls);
                                pg.Figures.Add(pfRight);
                                geometryGroup.Children.Add(pg);
                                DrawCommentCellsTriangle(shape, geometryGroup, r);
                            }

                            break;
                        }

                }
            }
#endif

        }
        #endregion
        //private bool allowDragColumns = false;
        //public bool AllowDragColumns
        //{
        //    get
        //    {
        //        return this.allowDragColumns;
        //    }

        //    set
        //    {
        //        var controller = this.MouseControllerDispatcher.Find(GridDragColumnHeaderMouseController.GridDragColumnHeaderMouseControllerName);
        //        if (value)
        //        {
        //            if (controller == null)
        //            {
        //                this.MouseControllerDispatcher.Add(new GridDragColumnHeaderMouseController(this));
        //            }
        //        }
        //        else
        //        {
        //            // if false and we found a controller then remove it
        //            if (controller != null)
        //            {
        //                this.MouseControllerDispatcher.Remove(controller);
        //            }
        //        }
        //        this.allowDragColumns = value;
        //    }
        //}
        //internal GridStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex)
        //{
        //    return GetRenderStyleInfo(rowIndex, columnIndex);
        //}

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
            if (line!=null)
            {
                return line.VisibleIndex;
            }
            return 0;
        }

        internal int GetRow(int currentRow)
        {
            VisibleLineInfo line = ScrollRows.GetVisibleLines()[currentRow];
            return line.LineIndex;
        }

        internal int GetCol(int currentCol)
        {
            VisibleLineInfo line = ScrollColumns.GetVisibleLines()[currentCol];
            return line.LineIndex;
        }
#if !WinRT
        /// <summary>
        /// Suspend formula calculation while rendering the GridControl
        /// </summary>
        public void SuspendFormulaCalculation()
        {
            if (this.Model != null)
            {
                this.Model.EnableFormulaCalculations = false;
            }
        }

        /// <summary>
        /// Resume formula calculation and refresh the range
        /// </summary>
        /// <param name="Range">Range to refresh</param>
        public void ResumeFormulaCalculation(GridRangeInfo Range)
        {
            if (this.Model != null)
            {
                this.Model.EnableFormulaCalculations = true;
                this.Model.FormulaEngine.RecalculateRange(Range, this.Model, false, true);
            }
        }
#endif
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

        public virtual void ScrollInView(RowColumnIndex cellRowColumnIndex)
        {
            if (this.ScrollOwner == null || !ScrollOwner.CanContentScroll)
            {
                Rect rect = RangeToClippedVisibleRect(cellRowColumnIndex, true);
                IScrollableInfo scp = GetScrollContentPresenter();
                //if (scp != null)
                //    scp.MakeVisible(this, rect);
            }
            else
            {
                ScrollRows.ScrollInView(cellRowColumnIndex.RowIndex);
                ScrollColumns.ScrollInView(cellRowColumnIndex.ColumnIndex);
            }
        }

        //public override void Dispose()
        //{
        //    if (this.cellRenderers != null)
        //    {
        //        this.cellRenderers.Dispose();
        //        this.cellRenderers = null;
        //    }
        //    base.Dispose();
        //}
        private IScrollableInfo GetScrollContentPresenter()
        {
            IScrollableInfo scp = (IScrollableInfo)GetParentTypeOf(this, typeof(IScrollableInfo));
            while (scp != null && !(scp.CanHorizontallyScroll || scp.CanVerticallyScroll))
            {
                DependencyObject dpo = (DependencyObject)scp;
                scp = (IScrollableInfo)GetParentTypeOf(VisualTreeHelper.GetParent(dpo), typeof(IScrollableInfo));
            }
            return scp;
        }

        DependencyObject GetParentTypeOf(DependencyObject dpo, Type type)
        {
#if !WinRT
            while (dpo != null && !type.IsAssignableFrom(dpo.GetType()))
#else
            while (dpo != null && !type.GetTypeInfo().IsAssignableFrom(dpo.GetType().GetTypeInfo()))
#endif
            {
                //if (dpo is Visual)
                dpo = VisualTreeHelper.GetParent(dpo);
                //else
                //    dpo = LogicalTreeHelper.GetParent(dpo);
            }
            return dpo;
        }

        public string PaneDesc
        {
            get { return ToString(); }
        }

        #region CurrentCell Events

        /// <summary>
        /// Implements handling for the PreviewMouseMove�event. When
        /// no mouse button is pressed and the mouse is over a cell it calls <see cref="DelayedCreateCellUIElements"/>.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        //protected override void OnPreviewMouseMove(MouseEventArgs e)
        //{
        //    RowColumnIndex rci = PointToCellRowColumnIndex(e);
        //    if (!rci.IsEmpty)
        //    {
        //        IGridCellRenderer renderer = RenderStyles[rci].CellRenderer;
        //        if (renderer != null)
        //            renderer.RaiseGridPreviewMouseMove(rci, e);
        //    }

        //    base.OnPreviewMouseMove(e);
        //}

        #region CurrentCellActivating

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
#if !WinRT
        [
        Description("Occurs before the grid activates the specified cell as current cell."),
        Category("Behavior")
        ]
#endif
        public event GridCurrentCellActivatingEventHandler CurrentCellActivating;

        internal bool RaiseCurrentCellActivating(ref RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            if (CurrentCell.IsSuspendEvents) return true;
            GridCurrentCellActivatingEventArgs e = new GridCurrentCellActivatingEventArgs(cellRowColumnIndex, activateOptions);
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
            if (CurrentCellActivating != null)
                CurrentCellActivating(this, e);
        }

        #endregion
        #region CurrentCellActivated

        internal void RaiseCurrentCellActivated()
        {
#if !WinRT
            //While deleting nested child record and parent record Message box will shown more than one times. To avoid this we have use the flag IsDeleteMessageBoxShown. This should reset after multi records deleted. 
            if ((this.Model is GridDataChildTableModel))
            {
                if (((GridDataChildTableModel)this.Model).ParentTable.Model!=null)
                    ((GridDataChildTableModel)this.Model).ParentTable.Model.CurrencyManager.IsDeleteMessageBoxShown = false;
               
            }
            else if (this.Model is GridDataTableModel)
            {
                ((GridDataTableModel)this.Model).CurrencyManager.IsDeleteMessageBoxShown = false;
            }
#endif
            if (CurrentCell.IsSuspendEvents) return;
            OnCurrentCellActivated();
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnCurrentCellActivated()
        {
            if (CurrentCellActivated != null)
                CurrentCellActivated(this, SyncfusionRoutedEventArgs.Empty);
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
        /// 
#if !WinRT
        [
        Description("Occurs after the grid activates the specified cell as current cell."),
        Category("Behavior")
        ]
#endif
        public event GridRoutedEventHandler CurrentCellActivated;
        #endregion
        #region CurrentCellActivateFailed

        internal void RaiseCurrentCellActivateFailed(RowColumnIndex cellRowColumnIndex)
        {
            if (CurrentCell.IsSuspendEvents) return;

            GridCurrentCellActivateFailedEventArgs e = new GridCurrentCellActivateFailedEventArgs(cellRowColumnIndex);
            OnCurrentCellActivateFailed(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivateFailed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="GridCurrentCellActivateFailedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCurrentCellActivateFailed(GridCurrentCellActivateFailedEventArgs e)
        {
            if (CurrentCellActivateFailed != null)
                CurrentCellActivateFailed(this, e);
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
        /// 
#if !WinRT
        [
        Description("Occurs after the grid fails to activate a specific cell as current cell."),
        Category("Behavior")
        ]
#endif
        public event GridCurrentCellActivateFailedEventHandler CurrentCellActivateFailed;
        #endregion
        #region CurrentCellDeactivating
        internal bool RaiseCurrentCellDeactivating()
        {
            if (CurrentCell.IsSuspendEvents) return true;

            SyncfusionCancelEventArgs e = new SyncfusionCancelEventArgs();
            this.OnCurrentCellDeactivating(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellDeactivating"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivating(SyncfusionCancelEventArgs e)
        {
            if (CurrentCellDeactivating != null)
                CurrentCellDeactivating(this, e);
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
#if !WinRT
        [
        Description("Occurs before the grid the deactivates the current cell."),
        Category("Behavior")
        ]
#endif
        public event SyncfusionCancelEventHandler CurrentCellDeactivating;
        #endregion
        #region CurrentCellDeactivated

        internal void RaiseCurrentCellDeactivated(RowColumnIndex cellRowColumnIndex)
        {
            if (CurrentCell.IsSuspendEvents) return;
            GridCurrentCellDeactivatedEventArgs e = new GridCurrentCellDeactivatedEventArgs(cellRowColumnIndex);
            this.OnCurrentCellDeactivated(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellDeactivated"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellDeactivatedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivated(GridCurrentCellDeactivatedEventArgs e)
        {
            if (CurrentCellDeactivated != null)
                CurrentCellDeactivated(this, e);
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
        /// 
#if !WinRT
        [
        Description("Occurs after the grid deactivates current cell."),
        Category("Behavior")
        ]
#endif
        public event GridCurrentCellDeactivatedEventHandler CurrentCellDeactivated;
        #endregion
        #region CurrentCellDeactivateFailed
        internal void RaiseCurrentCellDeactivateFailed()
        {
            if (CurrentCell.IsSuspendEvents) return;
            this.OnCurrentCellDeactivateFailed();
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellDeactivateFailed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivateFailed()
        {
            if (CurrentCellDeactivateFailed != null)
                CurrentCellDeactivateFailed(this, SyncfusionRoutedEventArgs.Empty);
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
        /// 
#if !WinRT
        [
        Description("Occurs after the grid fails to deactivate the current cell."),
        Category("Behavior")
        ]
#endif
        public event GridRoutedEventHandler CurrentCellDeactivateFailed;
        #endregion
        #region CurrentCellConfirmChangesFailed

        internal void RaiseCurrentCellConfirmChangesFailed()
        {
            if (CurrentCell.IsSuspendEvents) return;
            this.OnCurrentCellConfirmChangesFailed();
        }

        /// <summary>
        /// Raises the <see cref="CurrentCellConfirmChangesFailed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellConfirmChangesFailed()
        {
            if (CurrentCellConfirmChangesFailed != null)
                CurrentCellConfirmChangesFailed(this, SyncfusionRoutedEventArgs.Empty);
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
#if !WinRT
        [
        Description("Occurs when the grid accepted changes made to the active current cell."),
        Category("Behavior")
        ]
#endif
        public event GridRoutedEventHandler CurrentCellConfirmChangesFailed;
        #endregion
        #region CurrentCellAcceptedChanges

        internal bool RaiseCurrentCellAcceptedChanges()
        {
            if (CurrentCell.IsSuspendEvents) return true;
            this.OnCurrentCellAcceptedChanges();
            return true;// !e.Handled;
        }

        /// <summary>
        /// Raises the cancelable <see cref="GridControlBase.CurrentCellAcceptedChanges"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellAcceptedChanges()
        {
            if (CurrentCellAcceptedChanges != null)
                CurrentCellAcceptedChanges(this, SyncfusionRoutedEventArgs.Empty);
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
#if !WinRT
        [
        Description("Occurs when the grid accepted changes made to the active current cell."),
        Category("Behavior")
        ]
#endif
        public event GridRoutedEventHandler CurrentCellAcceptedChanges;
        #endregion
        #region CurrentCellChanging
        internal bool RaiseCurrentCellChanging()
        {
            if (CurrentCell.IsSuspendEvents) return true;

            SyncfusionCancelRoutedEventArgs e = new SyncfusionCancelRoutedEventArgs();
            this.OnCurrentCellChanging(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellChanging(SyncfusionCancelRoutedEventArgs e)
        {
            if (CurrentCellChanging != null)
                CurrentCellChanging(this, e);
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
#if !WinRT
        [
        Description("Occurs when the user wants to modify contents of the current cell."),
        Category("Behavior")
        ]
#endif
        public event GridCancelRoutedEventHandler CurrentCellChanging;
        #endregion
        #region CurrentCellStartEditing
        internal bool RaiseCurrentCellStartEditing()
        {
            if (CurrentCell.IsSuspendEvents) return true;

            SyncfusionCancelRoutedEventArgs e = new SyncfusionCancelRoutedEventArgs();
            this.OnCurrentCellStartEditing(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellStartEditing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellStartEditing(SyncfusionCancelRoutedEventArgs e)
        {
            if (CurrentCellStartEditing != null)
                CurrentCellStartEditing(this, e);
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
#if !WinRT
        [
        Description("Occurs before the current cell switches into editing mode."),
        Category("Behavior")
        ]
#endif
        public event GridCancelRoutedEventHandler CurrentCellStartEditing;
        #endregion
        #region CurrentCellEditingComplete
        internal void RaiseCurrentCellEditingComplete()
        {
            if (CurrentCell.IsSuspendEvents) return;
            this.OnCurrentCellEditingComplete(SyncfusionRoutedEventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellEditingComplete"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellEditingComplete(SyncfusionRoutedEventArgs e)
        {
            if (CurrentCellEditingComplete != null)
                CurrentCellEditingComplete(this, e);
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
#if !WinRT
        [
        Description("Occurs when the grid completes editing mode for the active current cell."),
        Category("Behavior")
        ]
#endif
        public event GridRoutedEventHandler CurrentCellEditingComplete;
        #endregion
        #region CurrentCellRejectedChanges
        internal void RaiseCurrentCellRejectedChanges()
        {
            if (CurrentCell.IsSuspendEvents) return;
            this.OnCurrentCellRejectedChanges(SyncfusionRoutedEventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellRejectedChanges"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellRejectedChanges(SyncfusionRoutedEventArgs e)
        {
            if (CurrentCellRejectedChanges != null)
                CurrentCellRejectedChanges(this, e);
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
#if !WinRT
        [
        Description("Occurs when the grid rejects changes made to the active current cell."),
        Category("Behavior")
        ]
#endif
        public event GridRoutedEventHandler CurrentCellRejectedChanges;
        #endregion
        #region CurrentCellChanged
        internal void RaiseCurrentCellChanged()
        {
            if (CurrentCell.IsSuspendEvents) return;
            ////Invalidating the cell to recalculate when the value exceeds the cell rect size
            if(CurrentCell != null && CurrentCell.Renderer != null && CurrentCell.Renderer.CurrentCellUIElement != null)
                if (MeasureText(CurrentCell.Renderer.ControlText, CurrentCell.Renderer.CurrentStyle).Width + 10 > CurrentCell.Renderer.CurrentCellUIElement.RenderSize.Width
                    && (CurrentCell.Renderer.CurrentStyle.EnableFloatCell == true || this.Model.Options.EnableFloatCell == true))
                {
                    this.InvalidateCell(new RowColumnIndex(CurrentCell.ColumnIndex, CurrentCell.RowIndex));
                    this.RenderCurrentCellBorder();
                }
            this.OnCurrentCellChanged(SyncfusionRoutedEventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellChanged(SyncfusionRoutedEventArgs e)
        {
            if (CurrentCellChanged != null)
                CurrentCellChanged(this, e);
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
#if !WinRT
        [
        Description("Occurs when the user changes contents of the current cell."),
        Category("Behavior")
        ]
#endif
        public event GridRoutedEventHandler CurrentCellChanged;
        #endregion
        #region CurrentCellMoved
        internal void RaiseCurrentCellMoved(GridActivateCurrentCellOptions activateOptions)
        {
            if (CurrentCell.IsSuspendEvents) return;

            GridCurrentCellMovedEventArgs e = new GridCurrentCellMovedEventArgs(activateOptions);
            this.OnCurrentCellMoved(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellMoved"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellMovedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellMoved(GridCurrentCellMovedEventArgs e)
        {
            if (CurrentCellMoved != null)
                CurrentCellMoved(this, e);
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
#if !WinRT
        [
        Description("Occurs when the current cell has been successfully moved to a new position."),
        Category("Behavior")
        ]
#endif
        public event GridCurrentCellMovedEventHandler CurrentCellMoved;
        #endregion
        #region CurrentCellMoveFailed
        internal void RaiseCurrentCellMoveFailed(RowColumnIndex rowColumnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            if (CurrentCell.IsSuspendEvents) return;

            GridCurrentCellMoveFailedEventArgs e = new GridCurrentCellMoveFailedEventArgs(rowColumnIndex, activateOptions);
            this.OnCurrentCellMoveFailed(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellMoveFailed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellMoveFailedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs e)
        {
            if (CurrentCellMoveFailed != null)
                CurrentCellMoveFailed(this, e);
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
#if !WinRT
        [
        Description("Occurs when the current cell fails to be moved to a new position."),
        Category("Behavior")
        ]
#endif
        public event GridCurrentCellMoveFailedEventHandler CurrentCellMoveFailed;
        #endregion
        #region CurrentCellMoving
        internal bool RaiseCurrentCellMoving(ref RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            if (CurrentCell.IsSuspendEvents) return true;

            GridCurrentCellMovingEventArgs e = new GridCurrentCellMovingEventArgs(cellRowColumnIndex, activateOptions);
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
            if (CurrentCellMoving != null)
                CurrentCellMoving(this, e);
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
#if !WinRT
        [
        Description("Occurs when the current cell is about to be moved to a new position."),
        Category("Behavior")
        ]
#endif
        public event GridCurrentCellMovingEventHandler CurrentCellMoving;
        #endregion
        #region CurrentCellValidating

        internal CurrentCellValidateEventArgs RaiseCurrentCellValidating(CurrentCellValidateEventArgs e)
        {
            if (CurrentCell.IsSuspendEvents) return null;
            this.OnCurrentCellValidating(e);
            return e;
        }

        internal bool RaiseCurrentCellValidating()
        {
            if (CurrentCell.IsSuspendEvents) return true;
            CurrentCellValidateEventArgs e = new CurrentCellValidateEventArgs();
            this.OnCurrentCellValidating(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the cancelable <see cref="GridControlBase.CurrentCellValidating"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CurrentCellValidateEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellValidating(CurrentCellValidateEventArgs e)
        {
            if (CurrentCellValidating != null)
                CurrentCellValidating(this, e);
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
#if !WinRT
        [
        Description("Occurs when the grid validates contents of the active current cell."),
        Category("Behavior")
        ]
#endif
        public event CurrentCellValidateEventHandler CurrentCellValidating;

        #endregion
        #region CurrentCellValidated
        internal void RaiseCurrentCellValidated()
        {
            if (CurrentCell.IsSuspendEvents) return;
            this.OnCurrentCellValidated(SyncfusionRoutedEventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellValidated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellValidated(SyncfusionRoutedEventArgs e)
        {
            if (CurrentCellValidated != null)
                CurrentCellValidated(this, e);
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
#if !WinRT
        [
        Description("Occurs when the grid validates contents of the active current cell."),
        Category("Behavior")
        ]
#endif
        public event GridRoutedEventHandler CurrentCellValidated;
        #endregion
        #region CurrentCellPreviewKeyDown

        protected virtual bool OnCurrentCellPreviewKeyDown(GridCellKeyEventArgs e)
        {
            var handler = this.CurrentCellKeyDown;
            if (handler != null)
            {
                handler(this, e);
            }

            return e.Handled;
        }

        /// <summary>
        /// Occurs during the PreviewKeyDown event for the current cell.
        /// </summary>
        public event GridCellKeyEventHandler CurrentCellKeyDown;
        #endregion
        #endregion

        #region CellEvents

        #region CellButtonClick
        internal void RaiseGridCellButtonClick(int rowIndex, int colIndex)
        {
            GridCellButtonClickEventArgs e = new GridCellButtonClickEventArgs(rowIndex, colIndex, this);
            this.OnCellButtonClick(e);
        }

        protected virtual void OnCellButtonClick(GridCellButtonClickEventArgs e)
        {
            if (CellButtonClick != null)
                CellButtonClick(this, e);
        }

        public event GridCellButtonClickEventHandler CellButtonClick;


        #endregion
        #region CellClick


        internal void RaiseGridCellClick(int rowIndex, int colIndex)
        {
            GridCellClickEventArgs e = new GridCellClickEventArgs(rowIndex, colIndex, this);
            this.OnCellClick(e);
        }

        protected virtual void OnCellClick(GridCellClickEventArgs e)
        {
            if (CellClick != null)
                CellClick(this, e);

        }

        public event GridCellClickEventHandler CellClick;

        #endregion
#if !WinRT
        #region CellCursor

        internal Cursor RaiseGridCellCursor()
        {
            GridCellCursorEventArgs e = new GridCellCursorEventArgs(this);
            if (e.Handled)
                return e.Cursor;
            return Cursors.Arrow;
        }

        protected virtual void OnGridCellCursor(GridCellCursorEventArgs e)
        {
            if (CellCursor != null)
                CellCursor(this, e);
        }

        public event GridCellCursorEventHandler CellCursor;

        #endregion
#endif
        #region CellMouseHoverEnter
        internal void RaiseCellMouseHoverEnter(MouseEventArgs e)
        {
            this.OnCellMouseHoverEnter(new GridCellMouseEventArgs(this)
                {
                    MouseEventArgs = e
                });
        }

        protected virtual void OnCellMouseHoverEnter(GridCellMouseEventArgs e)
        {
            if (CellMouseHoverEnter != null)
                CellMouseHoverEnter(this, e);
        }
        public event GridCellMouseEventHandler CellMouseHoverEnter;



        #endregion
        #region CellMouseHover
        internal void RaiseCellMouseHover(MouseControllerEventArgs e)
        {
            this.OnCellMouseHover(new GridCellMouseControllerEventArgs(this)
            {
                MouseControllerEventArgs = e
            });
        }

        protected virtual void OnCellMouseHover(GridCellMouseControllerEventArgs e)
        {
            if (CellMouseHover != null)
                CellMouseHover(this, e);
        }
        public event GridCellMouseControllerEventHandler CellMouseHover;



        #endregion
        #region CellMouseHoverLeave
        internal void RaiseCellMouseHoverLeave(MouseEventArgs e)
        {
            this.OnCellMouseHoverLeave(new GridCellMouseEventArgs(this)
            {
                MouseEventArgs = e
            });
        }

        protected virtual void OnCellMouseHoverLeave(GridCellMouseEventArgs e)
        {
            if (CellMouseHoverLeave != null)
                CellMouseHoverLeave(this, e);
        }
        public event GridCellMouseEventHandler CellMouseHoverLeave;



        #endregion
        #region CellMouseDown

        internal void RaiseCellMouseDown(MouseControllerEventArgs e)
        {
            this.OnCellMouseDown(new GridCellMouseControllerEventArgs(this)
            {
                MouseControllerEventArgs = e
            });
        }

        protected virtual void OnCellMouseDown(GridCellMouseControllerEventArgs e)
        {
            if (CellMouseDown != null)
                CellMouseDown(this, e);
        }


        public event GridCellMouseControllerEventHandler CellMouseDown;
        #endregion
        #region CellMouseMove
        internal void RaiseCellMouseMove(MouseControllerEventArgs e)
        {
            this.OnCellMouseMove(new GridCellMouseControllerEventArgs(this)
            {
                MouseControllerEventArgs = e
            });
        }

        protected virtual void OnCellMouseMove(GridCellMouseControllerEventArgs e)
        {
            if (CellMouseMove != null)
                CellMouseMove(this, e);
        }

        public event GridCellMouseControllerEventHandler CellMouseMove;

        #endregion
        #region CellMouseUp

        internal void RaiseCellMouseUp(MouseControllerEventArgs e)
        {
            this.OnCellMouseUp(new GridCellMouseControllerEventArgs(this)
            {
                MouseControllerEventArgs = e
            });
        }

        protected virtual void OnCellMouseUp(GridCellMouseControllerEventArgs e)
        {
            if (CellMouseUp != null)
                CellMouseUp(this, e);
        }
        public event GridCellMouseControllerEventHandler CellMouseUp;

        #endregion
        #region CellCancelMode

        internal void RaiseCellCancelMode()
        {
            this.OnCellCancelMode(new EventArgs());
        }

        protected virtual void OnCellCancelMode(EventArgs e)
        {
            if (CellCancelMode != null)
                CellCancelMode(this,e);
        }

        public event EventHandler CellCancelMode;

        #endregion
        #region CellRestoreMode

        internal void RaiseCellRestoreMode()
        {
            this.OnCellCancelMode(new EventArgs());
        }
        protected virtual void OnCellRestoreMode(EventArgs e)
        {
            if (CellRestoreMode != null)
                CellRestoreMode(this, e);
        }

        public event EventHandler CellRestoreMode;

        #endregion
        #region ResizingColumns


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
            GridResizingColumnsEventArgs e = new GridResizingColumnsEventArgs(this)
            {
                Columns = columns,
                Width = width,
                Reason = reason,
                Point = point,
                AllowResize = allowResize,
                InHiddenColResize = inHiddenColResize
            };
            this.OnResizingColumns(e);
            width = e.Width;
            return e.AllowResize;
        }

        internal bool RaiseResizingColumnsEvent(GridResizingColumnsEventArgs args)
        {
            this.OnResizingColumns(args);
            return args.AllowResize;
        }

        protected virtual void OnResizingColumns(GridResizingColumnsEventArgs e)
        {
            if (ResizingColumns != null)
                ResizingColumns(this, e);
        }

        public event GridResizingColumnsEventHandler ResizingColumns;

        #endregion
        #region ColumnDrag

        public event GridQueryDragColumnHeaderEventHandler QueryAllowDragColumn;

        internal bool RaiseQueryAllowDragColumn(int colIndex, int insertBeforeColumn, GridQueryDragColumnHeaderReason reason)
        {
            GridQueryDragColumnHeaderEventArgs ae = new GridQueryDragColumnHeaderEventArgs(colIndex, insertBeforeColumn, reason);
            this.OnRaiseQueryAllowDragColumn(ae);
            return ae.AllowDrag;
        }

        protected virtual void OnRaiseQueryAllowDragColumn(GridQueryDragColumnHeaderEventArgs ae)
        {
            var handler = this.QueryAllowDragColumn;
            if (handler != null)
            {
                handler(this, ae);
            }
        }

        #endregion
        #region ResizingRows

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
            GridResizingRowsEventArgs e = new GridResizingRowsEventArgs(this)
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

        internal bool RaiseResizingRowsEvent(GridResizingRowsEventArgs args)
        {
            this.OnResizingRows(args);
            return args.AllowResize;
        }

        protected virtual void OnResizingRows(GridResizingRowsEventArgs e)
        {
            if (ResizingRows != null)
                ResizingRows(this, e);
        }


        public event GridResizingRowsEventHandler ResizingRows;
        #endregion

        #endregion

        bool IsCurrentCellKeyboardFocusWithin
        {
            get
            {
                if (CurrentCell.HasCurrentCell && CurrentCell.Renderer.CurrentCellUIElement != null)
                    return true;// CurrentCell.Renderer.CurrentCellUIElement.IsKeyboardFocusWithin;
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
        /// <summary>
        /// Gets the custom tooltip template.
        /// </summary>
        /// <returns></returns>
        internal DataTemplate GetCustomTooltipTemplate()
        {
            var rd = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Grid.Silverlight;component/GridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };
            this.defaultTooltipTemplate = rd["customTooltipTemplate"] as DataTemplate;

            return this.defaultTooltipTemplate;
        }

        private DataTemplate defaultTooltipTemplate = null;
        internal DataTemplate GetDefaultTooltipTemplate()
        {
            if (this.defaultTooltipTemplate == null)
            {
#if !WinRT
                var stream = Application.GetResourceStream(new Uri("Syncfusion.Grid.Silverlight;component/GridControl/Themes/Generic.xaml", UriKind.Relative)).Stream;
#else
                var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Syncfusion.WinRT.Controls.Grid.GridControl.Themes.generic.xaml");
#endif
                var streamreader = new System.IO.StreamReader(stream);
                var rd = XamlReader.Load(streamreader.ReadToEnd()) as ResourceDictionary;
#if !WinRT
                streamreader.Close();
#endif
                this.defaultTooltipTemplate = rd["tooltipTemplate"] as DataTemplate;
            }

            return this.defaultTooltipTemplate;
        }

        private DataTemplate validationTemplate = null;
        internal DataTemplate GetDataValidationTemplate()
        {
            if (this.validationTemplate == null)
            {
#if !WinRT
                var stream = Application.GetResourceStream(new Uri("Syncfusion.Grid.Silverlight;component/GridControl/Themes/Generic.xaml", UriKind.Relative)).Stream;
#else
                var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Syncfusion.WinRT.Controls.Grid.GridControl.Themes.generic.xaml");
#endif
                var streamreader = new System.IO.StreamReader(stream);
                var rd = XamlReader.Load(streamreader.ReadToEnd()) as ResourceDictionary;
#if !WinRT
                streamreader.Close();
#endif
                this.validationTemplate = rd["datavalidationtooltipTemplate"] as DataTemplate;
            }

            return this.validationTemplate;
        }

        private DataTemplate defaultCommentTemplate = null;
        internal DataTemplate GetDefaultCommentTemplate()
        {
            if (this.defaultCommentTemplate == null)
            {
#if !WinRT
                var stream = Application.GetResourceStream(new Uri("Syncfusion.Grid.Silverlight;component/GridControl/Themes/Generic.xaml", UriKind.Relative)).Stream;
#else
                var names = this.GetType().GetTypeInfo().Assembly.GetManifestResourceNames();
                var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Syncfusion.WinRT.Controls.Grid.GridControl.Themes.generic.xaml");
#endif
                var streamreader = new System.IO.StreamReader(stream);
                var rd = XamlReader.Load(streamreader.ReadToEnd()) as ResourceDictionary;
#if !WinRT
                streamreader.Close();
#endif
                this.defaultCommentTemplate = rd["cellTemplate"] as DataTemplate;
            }

            return this.defaultCommentTemplate;
        }

        private DataTemplate defaultErrorTooltipTemplate = null;
        internal DataTemplate GetDefaultErrorTemplate()
        {
            if (this.defaultErrorTooltipTemplate == null)
            {
#if !WinRT
                var stream = Application.GetResourceStream(new Uri("Syncfusion.Grid.Silverlight;component/GridControl/Themes/Generic.xaml", UriKind.Relative)).Stream;
#else
                var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Syncfusion.WinRT.Controls.Grid.GridControl.Themes.generic.xaml");
#endif
                var streamreader = new System.IO.StreamReader(stream);
                var rd = XamlReader.Load(streamreader.ReadToEnd()) as ResourceDictionary;
#if  !WinRT
                streamreader.Close();
#endif
                this.defaultErrorTooltipTemplate = rd["errorTemplate"] as DataTemplate;
            }

            return this.defaultErrorTooltipTemplate;
        }

        internal static Canvas GetErrorMessageCanvas()
        {
            return XamlReader.Load(@"
      <Canvas Background=""White"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                              xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
        <Path Stretch=""Fill"" Height=""20"" Width=""20"" Canvas.Left=""0"" Canvas.Top=""0"" Data=""F1M53.265,84.651C32.514,84.651,15.693,101.472,15.693,122.222L15.693,122.222L15.693,184.192C15.693,204.941,32.514,221.763,53.265,221.763L53.265,221.763L116.572,221.763C137.322,221.763,154.143,204.941,154.143,184.192L154.143,184.192L154.143,122.222C154.143,101.472,137.322,84.651,116.572,84.651L116.572,84.651z"">
            <Path.Fill>
                <LinearGradientBrush EndPoint=""1,0.5"" StartPoint=""0,0.5"">
                    <GradientStop Color=""#FFF0090F"" Offset=""0""/>
                    <GradientStop Color=""#FFF0090F"" Offset=""1""/>
                </LinearGradientBrush>
            </Path.Fill>
        </Path>
        <Path Stretch=""Fill"" Height=""18"" Width=""18"" Canvas.Left=""1"" Canvas.Top=""1"" Data=""F1M53.265,89.577C35.265,89.577,20.621,104.222,20.621,122.222L20.621,122.222L20.621,184.192C20.621,202.192,35.265,216.836,53.265,216.836L53.265,216.836L116.572,216.836C134.572,216.836,149.215,202.192,149.215,184.192L149.215,184.192L149.215,122.222C149.215,104.222,134.572,89.577,116.572,89.577L116.572,89.577z"">
            <Path.Fill>
                <LinearGradientBrush EndPoint=""0.5,0"" StartPoint=""0.5,1"">
                    <GradientStop Color=""#FFF0090F"" Offset=""0""/>
                    <GradientStop Color=""#FFF8A1A2"" Offset=""1""/>
                </LinearGradientBrush>
            </Path.Fill>
        </Path>
        <Path Fill=""#FFB7130B"" Stretch=""Fill"" Height=""14"" Width=""14"" Canvas.Left=""3"" Canvas.Top=""3"" Data=""F1M117.4888,194.2466C113.7198,194.2466,109.9898,192.6546,107.2548,189.8796L88.7298,171.0826L69.9338,189.6066C67.1748,192.3276,63.5878,193.8266,59.8368,193.8266C56.3368,193.8266,53.0968,192.4936,50.7128,190.0746C45.6208,184.9076,45.9568,176.2886,51.4608,170.8626L70.2568,152.3386L51.7328,133.5426C49.1378,130.9096,47.6418,127.4866,47.5218,123.9036C47.3978,120.2256,48.7268,116.8226,51.2648,114.3216C53.6328,111.9876,56.8208,110.7026,60.2428,110.7026C64.0108,110.7026,67.7408,112.2946,70.4768,115.0696L89.0008,133.8666L107.7968,115.3426C110.5578,112.6216,114.1448,111.1226,117.8958,111.1226C121.3948,111.1226,124.6348,112.4546,127.0178,114.8736C132.1108,120.0416,131.7748,128.6606,126.2698,134.0856L107.4738,152.6106L125.9978,171.4066C128.5928,174.0386,130.0898,177.4626,130.2098,181.0456C130.3328,184.7236,129.0038,188.1266,126.4658,190.6276C124.0978,192.9616,120.9098,194.2466,117.4898,194.2466z""/>
        <Path Fill=""White"" Stretch=""Fill"" Height=""13"" Width=""13"" Canvas.Left=""3.5"" Canvas.Top=""3.5"" Data=""F1M103.1182,152.5786L124.1082,131.8926C128.4112,127.6516,128.7312,120.9996,124.8252,117.0356C120.9172,113.0706,114.2622,113.2946,109.9582,117.5366L88.9692,138.2216L68.2832,117.2316C64.0422,112.9286,57.3902,112.6086,53.4262,116.5146C49.4612,120.4226,49.6852,127.0786,53.9272,131.3816L74.6122,152.3706L53.6232,173.0556C49.3202,177.2976,48.9982,183.9476,52.9052,187.9126C56.8132,191.8766,63.4702,191.6546,67.7722,187.4126L88.7612,166.7276L109.4472,187.7176C113.6892,192.0206,120.3392,192.3416,124.3042,188.4346C128.2682,184.5266,128.0462,177.8706,123.8042,173.5686z""/>
    </Canvas> ") as Canvas;

        }

        internal static Canvas GetErrorInformationCavas()
        {
            return XamlReader.Load(@"
        <Canvas Background=""White""  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                              xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
            <Path Stretch=""Fill"" Height=""20"" Width=""20"" Canvas.Left=""0"" Canvas.Top=""0"" Data=""F1M187.111,154.774C187.111,192.023,217.307,222.219,254.556,222.219L254.556,222.219C291.804,222.219,322,192.023,322,154.774L322,154.774C322,117.525,291.804,87.329,254.556,87.329L254.556,87.329C217.307,87.329,187.111,117.525,187.111,154.774"">
                <Path.Fill>
                    <LinearGradientBrush EndPoint=""0.5,0"" StartPoint=""0.5,1"">
                        <GradientStop Color=""#FFF0090F"" Offset=""0""/>
                        <GradientStop Color=""#FFF0090F"" Offset=""1""/>
                    </LinearGradientBrush>
                </Path.Fill>
            </Path>
            <Path Stretch=""Fill"" Height=""18"" Width=""18"" Canvas.Left=""1"" Canvas.Top=""1"" Data=""F1M192.038,154.774C192.038,189.246,220.083,217.291,254.556,217.291L254.556,217.291C289.027,217.291,317.073,189.246,317.073,154.774L317.073,154.774C317.073,120.302,289.027,92.257,254.556,92.257L254.556,92.257C220.083,92.257,192.038,120.302,192.038,154.774"">
                <Path.Fill>
                    <LinearGradientBrush EndPoint=""0.5,0"" StartPoint=""0.5,1"">
                        <GradientStop Color=""#FFF0090F"" Offset=""0""/>
                        <GradientStop Color=""#FFF8A1A2"" Offset=""1""/>
                    </LinearGradientBrush>
                </Path.Fill>
            </Path>
            <Path Fill=""#FFB7130B"" Stretch=""Fill"" Height=""10"" Width=""5"" Canvas.Left=""8"" Canvas.Top=""2"" Data=""F1M256.5029,176.9707C252.2189,176.9707,248.7649,174.8147,247.0279,171.0537C245.9349,168.6807,245.2009,165.4447,244.7859,161.1577L242.9049,133.0397C242.5429,127.4127,242.3679,123.4557,242.3679,120.9347C242.3679,116.2507,243.7319,112.4177,246.4219,109.5427C249.2099,106.5617,252.8369,105.0497,257.2039,105.0497C264.6219,105.0497,267.7539,109.2007,268.9609,111.6767C270.4819,114.7987,271.2219,118.8777,271.2219,124.1457C271.2219,126.8947,271.0739,129.7197,270.7819,132.5397L268.2829,161.2217C267.9529,165.3577,267.1989,168.5907,265.9819,171.0807C264.1489,174.8237,260.6929,176.9707,256.5029,176.9707""/>
            <Path Fill=""White"" Stretch=""Fill"" Height=""10"" Width=""5"" Canvas.Left=""8"" Canvas.Top=""2"" Data=""F1M247.4756,160.8979C247.8606,164.8789,248.5166,167.8289,249.4806,169.9209C250.7756,172.7259,253.2696,174.2689,256.5026,174.2689C259.6696,174.2689,262.1736,172.7149,263.5546,169.8939C264.6216,167.7129,265.2886,164.7929,265.5916,160.9879L268.0956,132.2629C268.3776,129.5289,268.5196,126.7979,268.5196,124.1459C268.5196,119.2949,267.8706,115.6039,266.5346,112.8599C265.3976,110.5289,262.8556,107.7509,257.2046,107.7509C253.6156,107.7509,250.6516,108.9749,248.3946,111.3879C246.1876,113.7459,245.0706,116.9589,245.0706,120.9349C245.0706,123.3929,245.2436,127.2919,245.6006,132.8589z""/>
            <Path Fill=""#FFB7130B"" Stretch=""Fill"" Height=""5"" Width=""5"" Canvas.Left=""8"" Canvas.Top=""12.5"" Data=""F1M256.8535,207.7539C253.1005,207.7539,249.7715,206.5169,246.9585,204.0759C243.9565,201.4719,242.3685,197.7749,242.3685,193.3859C242.3685,189.4409,243.7765,186.0339,246.5515,183.2589C249.3265,180.4839,252.7525,179.0769,256.7365,179.0769C260.6955,179.0769,264.1265,180.4689,266.9355,183.2149C269.7795,185.9959,271.2215,189.4179,271.2215,193.3859C271.2215,197.7109,269.6615,201.3819,266.7085,204.0029C263.9105,206.4919,260.5925,207.7539,256.8535,207.7539""/>
            <Path Fill=""White"" Stretch=""Fill"" Height=""3"" Width=""3"" Canvas.Left=""9"" Canvas.Top=""13.5"" Data=""F1M256.7363,181.7778C253.4963,181.7778,250.7113,182.9188,248.4613,185.1698C246.2103,187.4198,245.0703,190.1838,245.0703,193.3858C245.0703,197.0188,246.3003,199.9288,248.7283,202.0358C251.0383,204.0388,253.7713,205.0538,256.8533,205.0538C259.9113,205.0538,262.6243,204.0208,264.9163,201.9838C267.3073,199.8588,268.5193,196.9668,268.5193,193.3858C268.5193,190.1718,267.3513,187.3998,265.0463,185.1458C262.7603,182.9108,259.9643,181.7778,256.7363,181.7778""/>
        </Canvas> ") as Canvas;
        }

        protected override void dispose()
        {
            if (Model != null)
            {
                UnwireModelEvents();
                model.CommitCellInfo -= new GridCommitCellInfoEventHandler(modelCommitCellInfo);
                model.Options.PropertyChanged -= new PropertyChangedEventHandler(Options_PropertyChanged);
                this.model.Dispose();
                this.model = null;

                if (this.renderStyles != null)
                {
                    this.renderStyles.Dispose();
                    this.renderStyles = null;
                }
                this.selectedCellsFrame = null;
                this.selectedCellsFrame = null;
                this.currentCellFrame = null;
                this.selectBorderFrame = null;
                this.dragBorderFrame = null;
                this.CellCommentFrame = null;
                this.hiddenBorderFrame = null;
                if (cellRenderers != null)
                {
                    cellRenderers.Dispose();
                    cellRenderers = null;
                }
                if (currentCell != null)
                {
                    currentCell.Dispose();
                    currentCell = null;
                }
                if (this.floatcellran != null)
                {
                    this.floatcellran.Clear();
                    this.floatcellran = null;
                }
#if !WinRT
                rowinPage.Clear();
                rowinPage = null;
                colinPage.Clear();
                colinPage = null;
                startRowinPage.Clear();
                startRowinPage = null;
                startColinPage.Clear();
                startColinPage = null;
#endif
            }
            base.dispose();
        }

        public event GridCellCommentOpeningEvent CellCommentOpening;

        public void RaiseCellCommentOpening(RowColumnIndex cell, ContentControl popup, string corner)
        {
            if (CellCommentOpening != null)
            {
                CellCommentOpening(this, new GridCellCommentOpeningEventArgs() { Cell = cell, Popup = popup, Corner = corner });
            }
        }
        public delegate void GridCellCommentOpeningEvent(object sender, GridCellCommentOpeningEventArgs e);
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellCommentOpeningEventArgs : EventArgs
    {
        public RowColumnIndex Cell { get; set; }
        public ContentControl Popup { get; set; }
        public string Corner { get; set; }
    }
}
