//-------------------------------------------------------------------------------------------------
// <copyright file="GridControl.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid.GridInternal;
#if SyncfusionFramework4_0
using System.Windows.Automation.Provider;
#elif SyncfusionFramework3_5
using System.Windows.Automation.Provider;
#endif

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// GridControl provides a simplified way to use a grid control and model in one place.
    /// </summary>
    /// <remarks>
    /// GridControl implements methods from <see cref="GridControlBase"/> and also implements all methods
    /// and events that are defined in <see cref="GridModel"/>. GridControl hides the implementation details
    /// that the underlying functionality is implemented in a separate model and view class and provides a one-stop
    /// interface to perform all grid operations in one place.<para/>
    /// GridControl also adds support for customizing the grid in a visual designer and lets you drop the grid control
    /// in a Windows Forms dialog at design-time.<para/>
    /// The <see cref="GridModel"/> can be attached and replaced with another object at run-time. The GridControl
    /// class will remove any dependencies on the old model and set up a relationship with the new model.
    /// </remarks>
    [ToolboxItem(true)]
    [System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Grid.GridControl), "ToolboxIcons.GridControl.bmp")]
    [Designer(typeof(Syncfusion.Windows.Forms.Grid.Design.GridControlDesigner))]
#if SyncfusionFramework2_0
    [Description("A cell-oriented grid."),
    DefaultEvent("CellClick"),
    Docking(DockingBehavior.Ask)]
#endif
    public class GridControl : GridControlBaseImp, ISupportInitialize,IVisualStyle
    {
        private ColorStyles colorStyles = ColorStyles.SystemTheme;
        private bool isMetroSettingsApplied = false;
        /// <overload>
        /// Initializes a new <see cref="GridControl"/> object with default settings.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridControl"/> object with default settings.
        /// </summary>
        public GridControl()
            : this(null)
        {
            InitializeModel();
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
            this.BeginUpdate();
            this.SuspendLayout();
            if (sf == 1.5)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    if (this.Model.RowHeights[i] != 0)
                        this.Model.RowHeights[i] += 5;
                }
                for (int i = 0; i < ColCount; i++)
                {
                    if (this.Model.ColWidths[i] != 0)
                        this.Model.ColWidths[i] += 15;
                }
            }
            else
            {
                for (int i = 0; i < RowCount; i++)
                {
                    if (this.Model.RowHeights[i] != 0)
                        this.Model.RowHeights[i] -= 5;
                }
                for (int i = 0; i < ColCount; i++)
                {
                    if (this.Model.ColWidths[i] != 0)
                    this.Model.ColWidths[i] -= 15;
                }
            }
            this.ResumeLayout();
            this.Invalidate();
            this.EndUpdate();
        } 
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }
        #endregion

        /// <override/>
        /// <summary>
        /// Gets or sets the site of the control.
        /// </summary>
        public override System.ComponentModel.ISite Site
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
        /// This is called when the grid has been assigned an empty model.
        /// </summary>
        internal void InitializeModel()
        {
            CommandStack.Enabled = false;
            Rows.DefaultSize = 17;
            Cols.DefaultSize = 65;
            RowHeights[0] = 25;
            ColWidths[0] = 35;
            RowHeights.ResetModified();
            ColWidths.ResetModified();
            ExcelLikeCurrentCell = false;
            ExcelLikeSelectionFrame = false;
            AllowDragSelectedCols = false;
            AllowDragSelectedRows = false;
            CommandStack.Enabled = false;
            FloatCellsMode = GridFloatCellsMode.None;

            ResetBaseStylesMap();

            GridModel model = Model;
            ////model.Properties.GridLineColor = Color.FromArgb(57, 73, 122);

            this.RowCount = 10;
            this.ColCount = 10;
        }
        
        /// <summary>
        /// Initializes a new <see cref="GridControl"/> and binds it to the specified <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">The grid model.</param>
        public GridControl(GridModel model)
            : base(model)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }

        /// <summary>
        /// Attaches a model for the grid control.
        /// </summary>
        /// <param name="model">The grid model to be attached.</param>
        /// <remarks>
        /// The <see cref="GridModel"/> can be attached and replaced with another object at run-time. The GridControl
        /// class will remove any dependencies on the old model and set up a relationship with the new model. <para/>
        /// When you replace the model, the grid control will completely refresh and draw the new cell data.
        /// </remarks>
        public void Attach(GridModel model)
        {
            Model = model;
        }
        
        /// <summary>
        /// Detaches a model from the grid control.
        /// </summary>
        /// <returns>The grid model.</returns>
        /// <remarks>
        /// The <see cref="GridModel"/> can be attached and replaced with another object at run-time. The GridControl
        /// class will remove any dependencies on the old model and set up a relationship with the new model. <para/>
        /// When you replace the model, the grid control will completely refresh and draw the new cell data.
        /// </remarks>
        public GridModel Detach()
        {
            GridModel model = Model;
            model = null;
            return model;
        }

        private bool hasFont = false;
        /// <override/>
        /// <summary>Specifies the font used to display text in the grid.</summary>
        [Description(@"The font used to display text in the grid."),
        AmbientValue(null),
        Category(@"Appearance")]
        [RefreshProperties(RefreshProperties.All)]
        public override Font Font
        {
            get
            {
                if (this.Parent != null && this.Parent.Font.Size != TableStyle.GdipFont.Size && !hasFont)
                {
                    return this.Parent.Font;
                }
                return TableStyle.GdipFont;
            }

            set
            {
                if (value != null)
                {
                    hasFont = true;
                    GridFontInfo font = TableStyle.Font;
                    font.Facename = value.FontFamily.Name;
                    font.FontStyle = value.Style;
                    font.Unit = value.Unit;
                    font.Size = value.Size;
                }
            }
        }

        private bool ShouldSerializeFont()
        {
            return TableStyle.HasFont;
        }

        /// <override/>
        /// <summary>Specifies the background image used for the grid.</summary>
        [Localizable(true),
        DefaultValue(null),
        Description(@"The background image used for the grid."),
        Category(@"Appearance"),
        RefreshProperties(RefreshProperties.Repaint)]
        public override Image BackgroundImage
        {
            set
            {
                base.BackgroundImage = value;
                if (!GetStyle(ControlStyles.SupportsTransparentBackColor) && value != null)
                {
                    Model.Options.TransparentBackground = true;
                }
            }

            get
            {
                return base.BackgroundImage;
            }
        }

        #region ModelEvents
        /// <override/>
        protected override void WireModel()
        {
            base.WireModel();

            // CellModelsChanged
            // QueryCellModel
            Model.CellModelsChanged += new CollectionChangeEventHandler(ModelCellModelsChanged);
            Model.QueryCellModel += new GridQueryCellModelEventHandler(ModelQueryCellModel);
            Model.Options.ControllerOptionsChanged += new EventHandler(ModelControllerOptionsChanged);
            Model.Options.DataObjectConsumerOptionsChanged += new EventHandler(ModelDataObjectConsumerOptionsChanged);
            Model.BaseStylesMapChanged += new EventHandler(ModelBaseStylesMapChanged);
            Model.DataChanged += new EventHandler(ModelDataChanged);
            Model.ModifiedChanged += new EventHandler(ModelModifiedChanged);
            Model.FileNameChanged += new EventHandler(ModelFileNameChanged);
            Model.CellsChanged += new GridCellsChangedEventHandler(ModelCellsChanged);
            Model.CellsChanging += new GridCellsChangingEventHandler(ModelCellsChanging);
            Model.ConfirmingPendingChanges += new CancelEventHandler(ModelConfirmingPendingChanges);
            Model.RefreshRequest += new EventHandler(ModelRefreshRequest);
            Model.BeginUpdateRequest += new EventHandler(ModelBeginUpdateRequest);
            Model.EndUpdateRequest += new GridEndUpdateRequestEventHandler(ModelEndUpdateRequest);
            Model.SaveCellInfo += new GridSaveCellInfoEventHandler(ModelSaveCellInfo);
            Model.PasteCellText += new GridPasteCellTextEventHandler(ModelPasteCellText);
            Model.QueryCellInfo += new GridQueryCellInfoEventHandler(ModelQueryCellInfo);
            Model.QueryCoveredRange += new GridQueryCoveredRangeEventHandler(ModelQueryCoveredRange);
            Model.QueryBanneredRange += new GridQueryBanneredRangeEventHandler(ModelQueryBanneredRange);
            Model.QueryColCount += new GridRowColCountEventHandler(ModelQueryColCount);
            Model.QueryRowCount += new GridRowColCountEventHandler(ModelQueryRowCount);
            Model.SaveColCount += new GridRowColCountEventHandler(ModelSaveColCount);
            Model.SaveRowCount += new GridRowColCountEventHandler(ModelSaveRowCount);
            ////Model.ChangingLayoutCells += new GridChangeLayoutCellsEventHandler(ModelChangingLayoutCells);
            ////Model.ChangedLayoutCells += new GridChangeLayoutCellsEventHandler(ModelChangedLayoutCells);
            Model.PrepareGraphics += new GraphicsEventHandler(ModelPrepareGraphics);
            Model.OperationFeedback += new OperationFeedbackEventHandler(ModelOperationFeedback);
            Model.RowHeightsChanged += new GridRowColSizeChangedEventHandler(ModelRowHeightsChanged);
            Model.RowHeightsChanging += new GridRowColSizeChangingEventHandler(ModelRowHeightsChanging);
            Model.ColWidthsChanged += new GridRowColSizeChangedEventHandler(ModelColWidthsChanged);
            Model.ColWidthsChanging += new GridRowColSizeChangingEventHandler(ModelColWidthsChanging);
            Model.QueryColWidth += new GridRowColSizeEventHandler(ModelQueryColWidth);
            Model.QueryRowHeight += new GridRowColSizeEventHandler(ModelQueryRowHeight);
            Model.SaveColWidth += new GridRowColSizeEventHandler(ModelSaveColWidth);
            Model.SaveRowHeight += new GridRowColSizeEventHandler(ModelSaveRowHeight);
            Model.ColsHidden += new GridRowColHiddenEventHandler(ModelColsHidden);
            Model.ColsHiding += new GridRowColHidingEventHandler(ModelColsHiding);
            Model.RowsHidden += new GridRowColHiddenEventHandler(ModelRowsHidden);
            Model.RowsHiding += new GridRowColHidingEventHandler(ModelRowsHiding);
            Model.QueryHideCol += new GridRowColHideEventHandler(ModelQueryHideCol);
            Model.SaveHideCol += new GridRowColHideEventHandler(ModelSaveHideCol);
            Model.QueryHideRow += new GridRowColHideEventHandler(ModelQueryHideRow);
            Model.SaveHideRow += new GridRowColHideEventHandler(ModelSaveHideRow);
            Model.DefaultRowHeightChanging += new GridDefaultSizeChangingEventHandler(ModelDefaultRowHeightChanging);
            Model.DefaultRowHeightChanged += new GridDefaultSizeChangedEventHandler(ModelDefaultRowHeightChanged);
            Model.HeaderRowCountChanged += new GridCountChangedEventHandler(ModelHeaderRowCountChanged);
            Model.HeaderRowCountChanging += new GridCountChangingEventHandler(ModelHeaderRowCountChanging);
            Model.FrozenRowCountChanged += new GridCountChangedEventHandler(ModelFrozenRowCountChanged);
            Model.FrozenRowCountChanging += new GridCountChangingEventHandler(ModelFrozenRowCountChanging);
            Model.RowsMoved += new GridRangeMovedEventHandler(ModelRowsMoved);
            Model.RowsMoving += new GridRangeMovingEventHandler(ModelRowsMoving);
            Model.RowsRemoved += new GridRangeRemovedEventHandler(ModelRowsRemoved);
            Model.RowsRemoving += new GridRangeRemovingEventHandler(ModelRowsRemoving);
            Model.RowsInserting += new GridRangeInsertingEventHandler(ModelRowsInserting);
            Model.RowsInserted += new GridRangeInsertedEventHandler(ModelRowsInserted);
            Model.DefaultColWidthChanging += new GridDefaultSizeChangingEventHandler(ModelDefaultColWidthChanging);
            Model.DefaultColWidthChanged += new GridDefaultSizeChangedEventHandler(ModelDefaultColWidthChanged);
            Model.HeaderColCountChanged += new GridCountChangedEventHandler(ModelHeaderColCountChanged);
            Model.HeaderColCountChanging += new GridCountChangingEventHandler(ModelHeaderColCountChanging);
            Model.FrozenColCountChanged += new GridCountChangedEventHandler(ModelFrozenColCountChanged);
            Model.FrozenColCountChanging += new GridCountChangingEventHandler(ModelFrozenColCountChanging);
            Model.ColsMoved += new GridRangeMovedEventHandler(ModelColsMoved);
            Model.ColsMoving += new GridRangeMovingEventHandler(ModelColsMoving);
            Model.ColsRemoved += new GridRangeRemovedEventHandler(ModelColsRemoved);
            Model.ColsRemoving += new GridRangeRemovingEventHandler(ModelColsRemoving);
            Model.ColsInserting += new GridRangeInsertingEventHandler(ModelColsInserting);
            Model.ColsInserted += new GridRangeInsertedEventHandler(ModelColsInserted);
            Model.ReadOnlyChanged += new EventHandler(ModelReadOnlyChanged);
            Model.SelectionChanging += new GridSelectionChangingEventHandler(ModelSelectionChanging);
            Model.SelectionChanged += new GridSelectionChangedEventHandler(ModelSelectionChanged);
            Model.PrepareClearSelection += new EventHandler(ModelPrepareClearSelection);
            Model.PrepareChangeSelection += new GridPrepareChangeSelectionEventHandler(ModelPrepareChangeSelection);
            Model.CoveredRangesChanging += new GridCoveredRangesChangingEventHandler(ModelCoveredRangesChanging);
            Model.CoveredRangesChanged += new GridCoveredRangesChangedEventHandler(ModelCoveredRangesChanged);
            Model.BanneredRangesChanging += new GridBanneredRangesChangingEventHandler(ModelBanneredRangesChanging);
            Model.BanneredRangesChanged += new GridBanneredRangesChangedEventHandler(ModelBanneredRangesChanged);
            ////            Model.FloatingCellsChanging += new GridFloatingCellsChangingEventHandler(ModelFloatingCellsChanging);
            Model.FloatingCellsChanged += new GridFloatingCellsChangedEventHandler(ModelFloatingCellsChanged);
            Model.MergeCellsChanged += new GridMergeCellsChangedEventHandler(ModelMergeCellsChanged);
            Model.QueryCanMergeCells += new GridQueryCanMergeCellsEventHandler(ModelQueryCanMergeCells);
            ////            Model.SynchronizeCurrentCell += new GridChangeLayoutCellsEventHandler(ModelSynchronizeCurrentCell);
        }
        
        /// <override/>
        protected override void UnwireModel()
        {
            base.UnwireModel();

            Model.CellModelsChanged -= new CollectionChangeEventHandler(ModelCellModelsChanged);
            Model.QueryCellModel -= new GridQueryCellModelEventHandler(ModelQueryCellModel);
            Model.Options.ControllerOptionsChanged -= new EventHandler(ModelControllerOptionsChanged);
            Model.Options.DataObjectConsumerOptionsChanged -= new EventHandler(ModelDataObjectConsumerOptionsChanged);
            Model.BaseStylesMapChanged -= new EventHandler(ModelBaseStylesMapChanged);
            Model.DataChanged -= new EventHandler(ModelDataChanged);
            Model.ModifiedChanged -= new EventHandler(ModelModifiedChanged);
            Model.FileNameChanged -= new EventHandler(ModelFileNameChanged);
            Model.CellsChanged -= new GridCellsChangedEventHandler(ModelCellsChanged);
            Model.CellsChanging -= new GridCellsChangingEventHandler(ModelCellsChanging);
            Model.ConfirmingPendingChanges -= new CancelEventHandler(ModelConfirmingPendingChanges);
            Model.RefreshRequest -= new EventHandler(ModelRefreshRequest);
            Model.BeginUpdateRequest -= new EventHandler(ModelBeginUpdateRequest);
            Model.EndUpdateRequest -= new GridEndUpdateRequestEventHandler(ModelEndUpdateRequest);
            Model.SaveCellInfo -= new GridSaveCellInfoEventHandler(ModelSaveCellInfo);
            Model.PasteCellText -= new GridPasteCellTextEventHandler(ModelPasteCellText);
            Model.QueryCellInfo -= new GridQueryCellInfoEventHandler(ModelQueryCellInfo);
            Model.QueryCoveredRange -= new GridQueryCoveredRangeEventHandler(ModelQueryCoveredRange);
            Model.QueryBanneredRange -= new GridQueryBanneredRangeEventHandler(ModelQueryBanneredRange);
            Model.QueryColCount -= new GridRowColCountEventHandler(ModelQueryColCount);
            Model.QueryRowCount -= new GridRowColCountEventHandler(ModelQueryRowCount);
            Model.SaveColCount -= new GridRowColCountEventHandler(ModelSaveColCount);
            Model.SaveRowCount -= new GridRowColCountEventHandler(ModelSaveRowCount);
            ////Model.ChangingLayoutCells -= new GridChangeLayoutCellsEventHandler(ModelChangingLayoutCells);
            ////Model.ChangedLayoutCells -= new GridChangeLayoutCellsEventHandler(ModelChangedLayoutCells);
            Model.PrepareGraphics -= new GraphicsEventHandler(ModelPrepareGraphics);
            Model.OperationFeedback -= new OperationFeedbackEventHandler(ModelOperationFeedback);
            Model.RowHeightsChanged -= new GridRowColSizeChangedEventHandler(ModelRowHeightsChanged);
            Model.RowHeightsChanging -= new GridRowColSizeChangingEventHandler(ModelRowHeightsChanging);
            Model.ColWidthsChanged -= new GridRowColSizeChangedEventHandler(ModelColWidthsChanged);
            Model.ColWidthsChanging -= new GridRowColSizeChangingEventHandler(ModelColWidthsChanging);
            Model.QueryColWidth -= new GridRowColSizeEventHandler(ModelQueryColWidth);
            Model.QueryRowHeight -= new GridRowColSizeEventHandler(ModelQueryRowHeight);
            Model.SaveColWidth -= new GridRowColSizeEventHandler(ModelSaveColWidth);
            Model.SaveRowHeight -= new GridRowColSizeEventHandler(ModelSaveRowHeight);
            Model.ColsHidden -= new GridRowColHiddenEventHandler(ModelColsHidden);
            Model.ColsHiding -= new GridRowColHidingEventHandler(ModelColsHiding);
            Model.RowsHidden -= new GridRowColHiddenEventHandler(ModelRowsHidden);
            Model.RowsHiding -= new GridRowColHidingEventHandler(ModelRowsHiding);
            Model.QueryHideCol -= new GridRowColHideEventHandler(ModelQueryHideCol);
            Model.SaveHideCol -= new GridRowColHideEventHandler(ModelSaveHideCol);
            Model.QueryHideRow -= new GridRowColHideEventHandler(ModelQueryHideRow);
            Model.SaveHideRow -= new GridRowColHideEventHandler(ModelSaveHideRow);
            Model.DefaultRowHeightChanging -= new GridDefaultSizeChangingEventHandler(ModelDefaultRowHeightChanging);
            Model.DefaultRowHeightChanged -= new GridDefaultSizeChangedEventHandler(ModelDefaultRowHeightChanged);
            Model.HeaderRowCountChanged -= new GridCountChangedEventHandler(ModelHeaderRowCountChanged);
            Model.HeaderRowCountChanging -= new GridCountChangingEventHandler(ModelHeaderRowCountChanging);
            Model.FrozenRowCountChanged -= new GridCountChangedEventHandler(ModelFrozenRowCountChanged);
            Model.FrozenRowCountChanging -= new GridCountChangingEventHandler(ModelFrozenRowCountChanging);
            Model.RowsMoved -= new GridRangeMovedEventHandler(ModelRowsMoved);
            Model.RowsMoving -= new GridRangeMovingEventHandler(ModelRowsMoving);
            Model.RowsRemoved -= new GridRangeRemovedEventHandler(ModelRowsRemoved);
            Model.RowsRemoving -= new GridRangeRemovingEventHandler(ModelRowsRemoving);
            Model.RowsInserting -= new GridRangeInsertingEventHandler(ModelRowsInserting);
            Model.RowsInserted -= new GridRangeInsertedEventHandler(ModelRowsInserted);
            Model.DefaultColWidthChanging -= new GridDefaultSizeChangingEventHandler(ModelDefaultColWidthChanging);
            Model.DefaultColWidthChanged -= new GridDefaultSizeChangedEventHandler(ModelDefaultColWidthChanged);
            Model.HeaderColCountChanged -= new GridCountChangedEventHandler(ModelHeaderColCountChanged);
            Model.HeaderColCountChanging -= new GridCountChangingEventHandler(ModelHeaderColCountChanging);
            Model.FrozenColCountChanged -= new GridCountChangedEventHandler(ModelFrozenColCountChanged);
            Model.FrozenColCountChanging -= new GridCountChangingEventHandler(ModelFrozenColCountChanging);
            Model.ColsMoved -= new GridRangeMovedEventHandler(ModelColsMoved);
            Model.ColsMoving -= new GridRangeMovingEventHandler(ModelColsMoving);
            Model.ColsRemoved -= new GridRangeRemovedEventHandler(ModelColsRemoved);
            Model.ColsRemoving -= new GridRangeRemovingEventHandler(ModelColsRemoving);
            Model.ColsInserting -= new GridRangeInsertingEventHandler(ModelColsInserting);
            Model.ColsInserted -= new GridRangeInsertedEventHandler(ModelColsInserted);
            Model.ReadOnlyChanged -= new EventHandler(ModelReadOnlyChanged);
            Model.SelectionChanging -= new GridSelectionChangingEventHandler(ModelSelectionChanging);
            Model.SelectionChanged -= new GridSelectionChangedEventHandler(ModelSelectionChanged);
            Model.PrepareClearSelection -= new EventHandler(ModelPrepareClearSelection);
            Model.PrepareChangeSelection -= new GridPrepareChangeSelectionEventHandler(ModelPrepareChangeSelection);
            Model.CoveredRangesChanging -= new GridCoveredRangesChangingEventHandler(ModelCoveredRangesChanging);
            Model.CoveredRangesChanged -= new GridCoveredRangesChangedEventHandler(ModelCoveredRangesChanged);
            Model.BanneredRangesChanging -= new GridBanneredRangesChangingEventHandler(ModelBanneredRangesChanging);
            Model.BanneredRangesChanged -= new GridBanneredRangesChangedEventHandler(ModelBanneredRangesChanged);
            ////            Model.FloatingCellsChanging -= new GridFloatingCellsChangingEventHandler(ModelFloatingCellsChanging);
            Model.FloatingCellsChanged -= new GridFloatingCellsChangedEventHandler(ModelFloatingCellsChanged);
            ////            Model.SynchronizeCurrentCell -= new GridChangeLayoutCellsEventHandler(ModelSynchronizeCurrentCell);
            Model.MergeCellsChanged -= new GridMergeCellsChangedEventHandler(ModelMergeCellsChanged);
            Model.QueryCanMergeCells -= new GridQueryCanMergeCellsEventHandler(ModelQueryCanMergeCells);
        }
        
        //// Model Events
        void ModelCellModelsChanged(object sender, CollectionChangeEventArgs e)
        {
            OnCellModelsChanged(e);
        }

        void ModelQueryCellModel(object sender, GridQueryCellModelEventArgs e)
        {
            OnQueryCellModel(e);
        }

        void ModelFloatingCellsChanged(object sender, GridFloatingCellsChangedEventArgs e)
        {
            OnFloatingCellsChanged(e);
        }

        void ModelMergeCellsChanged(object sender, GridMergeCellsChangedEventArgs e)
        {
            OnMergeCellsChanged(e);
        }

        void ModelQueryCanMergeCells(object sender, GridQueryCanMergeCellsEventArgs e)
        {
            OnQueryCanMergeCells(e);
        }

        void ModelCoveredRangesChanged(object sender, GridCoveredRangesChangedEventArgs e)
        {
            OnCoveredRangesChanged(e);
        }

        void ModelCoveredRangesChanging(object sender, GridCoveredRangesChangingEventArgs e)
        {
            OnCoveredRangesChanging(e);
        }

        void ModelBanneredRangesChanged(object sender, GridBanneredRangesChangedEventArgs e)
        {
            OnBanneredRangesChanged(e);
        }

        void ModelBanneredRangesChanging(object sender, GridBanneredRangesChangingEventArgs e)
        {
            OnBanneredRangesChanging(e);
        }

        void ModelPrepareChangeSelection(object sender, GridPrepareChangeSelectionEventArgs e)
        {
            OnPrepareChangeSelection(e);
        }

        void ModelPrepareClearSelection(object sender, EventArgs e)
        {
            OnPrepareClearSelection(e);
        }

        void ModelSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            OnSelectionChanged(e);
        }

        void ModelSelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            OnSelectionChanging(e);
        }

        void ModelConfirmingPendingChanges(object sender, CancelEventArgs e)
        {
            OnConfirmingPendingChanges(e);
        }

        void ModelSaveRowCount(object sender, GridRowColCountEventArgs e)
        {
            OnSaveRowCount(e);
        }

        void ModelSaveColCount(object sender, GridRowColCountEventArgs e)
        {
            OnSaveColCount(e);
        }

        void ModelQueryRowCount(object sender, GridRowColCountEventArgs e)
        {
            OnQueryRowCount(e);
        }

        void ModelQueryColCount(object sender, GridRowColCountEventArgs e)
        {
            OnQueryColCount(e);
        }

        void ModelCellsChanged(object sender, GridCellsChangedEventArgs e)
        {
            OnCellsChanged(e);
        }

        void ModelCellsChanging(object sender, GridCellsChangingEventArgs e)
        {
            OnCellsChanging(e);
        }

        void ModelSaveCellInfo(object sender, GridSaveCellInfoEventArgs e)
        {
            OnSaveCellInfo(e);
        }

        void ModelPasteCellText(object sender, GridPasteCellTextEventArgs e)
        {
            OnPasteCellText(e);
        }

        void ModelColsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            OnColsInserted(e);
        }

        void ModelColsInserting(object sender, GridRangeInsertingEventArgs e)
        {
            OnColsInserting(e);
        }

        void ModelColsRemoving(object sender, GridRangeRemovingEventArgs e)
        {
            OnColsRemoving(e);
        }

        void ModelColsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            OnColsRemoved(e);
        }

        void ModelColsMoving(object sender, GridRangeMovingEventArgs e)
        {
            OnColsMoving(e);
        }

        void ModelColsMoved(object sender, GridRangeMovedEventArgs e)
        {
            OnColsMoved(e);
        }

        void ModelFrozenColCountChanging(object sender, GridCountChangingEventArgs e)
        {
            OnFrozenColCountChanging(e);
        }

        void ModelFrozenColCountChanged(object sender, GridCountChangedEventArgs e)
        {
            OnFrozenColCountChanged(e);
        }

        void ModelHeaderColCountChanging(object sender, GridCountChangingEventArgs e)
        {
            OnHeaderColCountChanging(e);
        }

        void ModelHeaderColCountChanged(object sender, GridCountChangedEventArgs e)
        {
            OnHeaderColCountChanged(e);
        }

        void ModelDefaultColWidthChanged(object sender, GridDefaultSizeChangedEventArgs e)
        {
            OnDefaultColWidthChanged(e);
        }

        void ModelDefaultColWidthChanging(object sender, GridDefaultSizeChangingEventArgs e)
        {
            OnDefaultColWidthChanging(e);
        }

        void ModelRowsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            OnRowsInserted(e);
        }

        void ModelRowsInserting(object sender, GridRangeInsertingEventArgs e)
        {
            OnRowsInserting(e);
        }

        void ModelRowsRemoving(object sender, GridRangeRemovingEventArgs e)
        {
            OnRowsRemoving(e);
        }

        void ModelRowsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            OnRowsRemoved(e);
        }

        void ModelRowsMoving(object sender, GridRangeMovingEventArgs e)
        {
            OnRowsMoving(e);
        }

        void ModelRowsMoved(object sender, GridRangeMovedEventArgs e)
        {
            OnRowsMoved(e);
        }

        void ModelFrozenRowCountChanging(object sender, GridCountChangingEventArgs e)
        {
            OnFrozenRowCountChanging(e);
        }

        void ModelFrozenRowCountChanged(object sender, GridCountChangedEventArgs e)
        {
            OnFrozenRowCountChanged(e);
        }

        void ModelHeaderRowCountChanging(object sender, GridCountChangingEventArgs e)
        {
            OnHeaderRowCountChanging(e);
        }

        void ModelHeaderRowCountChanged(object sender, GridCountChangedEventArgs e)
        {
            OnHeaderRowCountChanged(e);
        }

        void ModelDefaultRowHeightChanged(object sender, GridDefaultSizeChangedEventArgs e)
        {
            OnDefaultRowHeightChanged(e);
        }

        void ModelDefaultRowHeightChanging(object sender, GridDefaultSizeChangingEventArgs e)
        {
            OnDefaultRowHeightChanging(e);
        }

        void ModelQueryHideCol(object sender, GridRowColHideEventArgs e)
        {
            OnQueryHideCol(e);
        }

        void ModelSaveHideCol(object sender, GridRowColHideEventArgs e)
        {
            OnSaveHideCol(e);
        }

        void ModelQueryHideRow(object sender, GridRowColHideEventArgs e)
        {
            OnQueryHideRow(e);
        }

        void ModelSaveHideRow(object sender, GridRowColHideEventArgs e)
        {
            OnSaveHideRow(e);
        }

        void ModelColsHiding(object sender, GridRowColHidingEventArgs e)
        {
            OnColsHiding(e);
        }

        void ModelColsHidden(object sender, GridRowColHiddenEventArgs e)
        {
            OnColsHidden(e);
        }

        void ModelRowsHiding(object sender, GridRowColHidingEventArgs e)
        {
            OnRowsHiding(e);
        }

        void ModelRowsHidden(object sender, GridRowColHiddenEventArgs e)
        {
            OnRowsHidden(e);
        }

        void ModelQueryColWidth(object sender, GridRowColSizeEventArgs e)
        {
            OnQueryColWidth(e);
        }

        void ModelSaveColWidth(object sender, GridRowColSizeEventArgs e)
        {
            OnSaveColWidth(e);
        }

        void ModelQueryRowHeight(object sender, GridRowColSizeEventArgs e)
        {
            OnQueryRowHeight(e);
        }

        void ModelSaveRowHeight(object sender, GridRowColSizeEventArgs e)
        {
            OnSaveRowHeight(e);
        }

        void ModelColWidthsChanging(object sender, GridRowColSizeChangingEventArgs e)
        {
            OnColWidthsChanging(e);
        }

        void ModelColWidthsChanged(object sender, GridRowColSizeChangedEventArgs e)
        {
            OnColWidthsChanged(e);
        }

        void ModelRowHeightsChanging(object sender, GridRowColSizeChangingEventArgs e)
        {
            OnRowHeightsChanging(e);
        }

        void ModelRowHeightsChanged(object sender, GridRowColSizeChangedEventArgs e)
        {
            OnRowHeightsChanged(e);
        }

        void ModelDataChanged(object sender, EventArgs e)
        {
            OnDataChanged(e);
        }

        void ModelModifiedChanged(object sender, EventArgs e)
        {
            OnModifiedChanged(e);
        }

        void ModelFileNameChanged(object sender, EventArgs e)
        {
            OnFileNameChanged(e);
        }

        void ModelBeginUpdateRequest(object sender, EventArgs e)
        {
            OnBeginUpdateRequest(e);
        }

        void ModelEndUpdateRequest(object sender, GridEndUpdateRequestEventArgs e)
        {
            OnEndUpdateRequest(e);
        }
        
        void ModelQueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            OnQueryCellInfo(e);
        }

        void ModelQueryCoveredRange(object sender, GridQueryCoveredRangeEventArgs e)
        {
            OnQueryCoveredRange(e);
        }

        void ModelQueryBanneredRange(object sender, GridQueryBanneredRangeEventArgs e)
        {
            OnQueryBanneredRange(e);
        }

        void ModelChangingLayoutCells(object sender, GridChangeLayoutCellsEventArgs e)
        {
            OnChangingLayoutCells(e);
        }

        void ModelChangedLayoutCells(object sender, GridChangeLayoutCellsEventArgs e)
        {
            OnChangedLayoutCells(e);
        }

        void ModelPrepareGraphics(object sender, GraphicsEventArgs e)
        {
            OnPrepareGraphics(e);
        }

        void ModelOperationFeedback(object sender, OperationFeedbackEventArgs e)
        {
            OnOperationFeedback(e);
        }

        void ModelReadOnlyChanged(object sender, EventArgs e)
        {
            OnReadOnlyChanged(e);
        }

        void ModelControllerOptionsChanged(object sender, EventArgs e)
        {
            OnControllerOptionsChanged(e);
        }

        void ModelDataObjectConsumerOptionsChanged(object sender, EventArgs e)
        {
            this.OnDataObjectConsumerOptionsChanged(e);
        }

        void ModelBaseStylesMapChanged(object sender, EventArgs e)
        {
            OnBaseStylesMapChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ControllerOptionsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnControllerOptionsChanged(EventArgs e)
        {
            if (ControllerOptionsChanged != null)
            {
                ControllerOptionsChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.DataObjectConsumerOptionsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnDataObjectConsumerOptionsChanged(EventArgs e)
        {
            if (DataObjectConsumerOptionsChanged != null)
            {
                DataObjectConsumerOptionsChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.BaseStylesMapChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnBaseStylesMapChanged(EventArgs e)
        {
            if (BaseStylesMapChanged != null)
            {
                BaseStylesMapChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.DataChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnDataChanged(EventArgs e)
        {
            if (DataChanged != null)
            {
                DataChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ModifiedChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnModifiedChanged(EventArgs e)
        {
            if (ModifiedChanged != null)
            {
                ModifiedChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.FileNameChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnFileNameChanged(EventArgs e)
        {
            if (FileNameChanged != null)
            {
                FileNameChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.CellsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellsChanged(GridCellsChangedEventArgs e)
        {
            if (CellsChanged != null)
            {
                CellsChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.CellsChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellsChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellsChanging(GridCellsChangingEventArgs e)
        {
            if (CellsChanging != null)
            {
                CellsChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ConfirmingPendingChanges"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnConfirmingPendingChanges(CancelEventArgs e)
        {
            if (ConfirmingPendingChanges != null)
            {
                ConfirmingPendingChanges(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RefreshRequest"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnRefreshRequest(EventArgs e)
        {
            if (RefreshRequest != null)
            {
                RefreshRequest(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.BeginUpdateRequest"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnBeginUpdateRequest(EventArgs e)
        {
            if (BeginUpdateRequest != null)
            {
                BeginUpdateRequest(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.EndUpdateRequest"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridEndUpdateRequestEventArgs" /> that contains the event data.</param>
        protected virtual void OnEndUpdateRequest(GridEndUpdateRequestEventArgs e)
        {
            if (EndUpdateRequest != null)
            {
                EndUpdateRequest(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.SaveCellInfo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSaveCellInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveCellInfo(GridSaveCellInfoEventArgs e)
        {
            if (SaveCellInfo != null)
            {
                SaveCellInfo(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.PasteCellText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSaveCellInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnPasteCellText(GridPasteCellTextEventArgs e)
        {
            if (PasteCellText != null)
            {
                PasteCellText(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.QueryCellInfo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCellInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            if (QueryCellInfo != null)
            {
                QueryCellInfo(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.QueryCoveredRange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCoveredRangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            if (QueryCoveredRange != null)
            {
                QueryCoveredRange(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.QueryBanneredRange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryBanneredRangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryBanneredRange(GridQueryBanneredRangeEventArgs e)
        {
            if (QueryBanneredRange != null)
            {
                QueryBanneredRange(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.QueryColCount"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColCountEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryColCount(GridRowColCountEventArgs e)
        {
            if (QueryColCount != null)
            {
                QueryColCount(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.QueryRowCount"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColCountEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryRowCount(GridRowColCountEventArgs e)
        {
            if (QueryRowCount != null)
            {
                QueryRowCount(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.SaveColCount"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColCountEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveColCount(GridRowColCountEventArgs e)
        {
            if (SaveColCount != null)
            {
                SaveColCount(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.SaveRowCount"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColCountEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveRowCount(GridRowColCountEventArgs e)
        {
            if (SaveRowCount != null)
            {
                SaveRowCount(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ChangingLayoutCells"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridChangeLayoutCellsEventArgs" /> that contains the event data.</param>
        internal void OnChangingLayoutCells(GridChangeLayoutCellsEventArgs e)
        {
            if (ChangingLayoutCells != null)
            {
                ChangingLayoutCells(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ChangedLayoutCells"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridChangeLayoutCellsEventArgs" /> that contains the event data.</param>
        internal void OnChangedLayoutCells(GridChangeLayoutCellsEventArgs e)
        {
            if (ChangedLayoutCells != null)
            {
                ChangedLayoutCells(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.PrepareGraphics"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GraphicsEventArgs" /> that contains the event data.</param>
        protected virtual void OnPrepareGraphics(GraphicsEventArgs e)
        {
            if (PrepareGraphics != null)
            {
                PrepareGraphics(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.OperationFeedback"/> event.
        /// </summary>
        /// <param name="e">A <see cref="OperationFeedbackEventArgs" /> that contains the event data.</param>
        protected virtual void OnOperationFeedback(OperationFeedbackEventArgs e)
        {
            if (OperationFeedback != null)
            {
                OperationFeedback(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RowHeightsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowHeightsChanged(GridRowColSizeChangedEventArgs e)
        {
            if (RowHeightsChanged != null)
            {
                RowHeightsChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RowHeightsChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowHeightsChanging(GridRowColSizeChangingEventArgs e)
        {
            if (RowHeightsChanging != null)
            {
                RowHeightsChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ColWidthsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnColWidthsChanged(GridRowColSizeChangedEventArgs e)
        {
            if (ColWidthsChanged != null)
            {
                ColWidthsChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ColWidthsChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnColWidthsChanging(GridRowColSizeChangingEventArgs e)
        {
            if (ColWidthsChanging != null)
            {
                ColWidthsChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.QueryColWidth"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryColWidth(GridRowColSizeEventArgs e)
        {
            if (QueryColWidth != null)
            {
                QueryColWidth(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.QueryRowHeight"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryRowHeight(GridRowColSizeEventArgs e)
        {
            if (QueryRowHeight != null)
            {
                QueryRowHeight(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.SaveColWidth"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveColWidth(GridRowColSizeEventArgs e)
        {
            if (SaveColWidth != null)
            {
                SaveColWidth(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.SaveRowHeight"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColSizeEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveRowHeight(GridRowColSizeEventArgs e)
        {
            if (SaveRowHeight != null)
            {
                SaveRowHeight(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ColsHidden"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHiddenEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsHidden(GridRowColHiddenEventArgs e)
        {
            if (ColsHidden != null)
            {
                ColsHidden(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ColsHiding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHidingEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsHiding(GridRowColHidingEventArgs e)
        {
            if (ColsHiding != null)
            {
                ColsHiding(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RowsHidden"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHiddenEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsHidden(GridRowColHiddenEventArgs e)
        {
            if (RowsHidden != null)
            {
                RowsHidden(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RowsHiding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHidingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsHiding(GridRowColHidingEventArgs e)
        {
            if (RowsHiding != null)
            {
                RowsHiding(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.QueryHideCol"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHideEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryHideCol(GridRowColHideEventArgs e)
        {
            if (QueryHideCol != null)
            {
                QueryHideCol(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.SaveHideCol"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHideEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveHideCol(GridRowColHideEventArgs e)
        {
            if (SaveHideCol != null)
            {
                SaveHideCol(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.QueryHideRow"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHideEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryHideRow(GridRowColHideEventArgs e)
        {
            if (QueryHideRow != null)
            {
                QueryHideRow(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.SaveHideRow"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHideEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveHideRow(GridRowColHideEventArgs e)
        {
            if (SaveHideRow != null)
            {
                SaveHideRow(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.DefaultRowHeightChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDefaultSizeChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnDefaultRowHeightChanging(GridDefaultSizeChangingEventArgs e)
        {
            if (DefaultRowHeightChanging != null)
            {
                DefaultRowHeightChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.DefaultRowHeightChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDefaultSizeChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnDefaultRowHeightChanged(GridDefaultSizeChangedEventArgs e)
        {
            if (DefaultRowHeightChanged != null)
            {
                DefaultRowHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.HeaderRowCountChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnHeaderRowCountChanged(GridCountChangedEventArgs e)
        {
            if (HeaderRowCountChanged != null)
            {
                HeaderRowCountChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.HeaderRowCountChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnHeaderRowCountChanging(GridCountChangingEventArgs e)
        {
            if (HeaderRowCountChanging != null)
            {
                HeaderRowCountChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.FrozenRowCountChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnFrozenRowCountChanged(GridCountChangedEventArgs e)
        {
            if (FrozenRowCountChanged != null)
            {
                FrozenRowCountChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.FrozenRowCountChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnFrozenRowCountChanging(GridCountChangingEventArgs e)
        {
            if (FrozenRowCountChanging != null)
            {
                FrozenRowCountChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RowsMoved"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeMovedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsMoved(GridRangeMovedEventArgs e)
        {
            if (RowsMoved != null)
            {
                RowsMoved(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RowsMoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeMovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsMoving(GridRangeMovingEventArgs e)
        {
            if (RowsMoving != null)           
            { 
                RowsMoving(this, e); 
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RowsRemoved"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeRemovedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsRemoved(GridRangeRemovedEventArgs e)
        {
            if (RowsRemoved != null)
            {
                RowsRemoved(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RowsRemoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeRemovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsRemoving(GridRangeRemovingEventArgs e)
        {
            if (RowsRemoving != null)
            {
                RowsRemoving(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RowsInserting"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeInsertingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsInserting(GridRangeInsertingEventArgs e)
        {
            if (RowsInserting != null)
            {
                RowsInserting(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.RowsInserted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeInsertedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsInserted(GridRangeInsertedEventArgs e)
        {
            if (RowsInserted != null)
            {
                RowsInserted(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.DefaultColWidthChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDefaultSizeChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnDefaultColWidthChanging(GridDefaultSizeChangingEventArgs e)
        {
            if (DefaultColWidthChanging != null)
            {
                DefaultColWidthChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.DefaultColWidthChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDefaultSizeChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnDefaultColWidthChanged(GridDefaultSizeChangedEventArgs e)
        {
            if (DefaultColWidthChanged != null)
            {
                DefaultColWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.HeaderColCountChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnHeaderColCountChanged(GridCountChangedEventArgs e)
        {
            if (HeaderColCountChanged != null)
            {
                HeaderColCountChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.HeaderColCountChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnHeaderColCountChanging(GridCountChangingEventArgs e)
        {
            if (HeaderColCountChanging != null)
            {
                HeaderColCountChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.FrozenColCountChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnFrozenColCountChanged(GridCountChangedEventArgs e)
        {
            if (FrozenColCountChanged != null)
            {
                FrozenColCountChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.FrozenColCountChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCountChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnFrozenColCountChanging(GridCountChangingEventArgs e)
        {
            if (FrozenColCountChanging != null)
            {
                FrozenColCountChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ColsMoved"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeMovedEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsMoved(GridRangeMovedEventArgs e)
        {
            if (ColsMoved != null)
            {
                ColsMoved(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ColsMoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeMovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsMoving(GridRangeMovingEventArgs e)
        {
            if (ColsMoving != null)
            {
                ColsMoving(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ColsRemoved"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeRemovedEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsRemoved(GridRangeRemovedEventArgs e)
        {
            if (ColsRemoved != null)
            {
                ColsRemoved(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ColsRemoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeRemovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsRemoving(GridRangeRemovingEventArgs e)
        {
            if (ColsRemoving != null)
            {
                ColsRemoving(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ColsInserting"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeInsertingEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsInserting(GridRangeInsertingEventArgs e)
        {
            if (ColsInserting != null)
            {
                ColsInserting(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ColsInserted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRangeInsertedEventArgs" /> that contains the event data.</param>
        protected virtual void OnColsInserted(GridRangeInsertedEventArgs e)
        {
            if (ColsInserted != null)
            {
                ColsInserted(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.ReadOnlyChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnReadOnlyChanged(EventArgs e)
        {
            if (ReadOnlyChanged != null)
            {
                ReadOnlyChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.SelectionChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectionChanging(GridSelectionChangingEventArgs e)
        {
            if (SelectionChanging != null)
            {
                SelectionChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.SelectionChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectionChanged(GridSelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.PrepareClearSelection"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnPrepareClearSelection(EventArgs e)
        {
            if (PrepareClearSelection != null)
            {
                PrepareClearSelection(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.QueryCellModel"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCellModelEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellModel(GridQueryCellModelEventArgs e)
        {
            if (QueryCellModel != null)
            {
                QueryCellModel(this, e);
            }

            if (e.CellModel == null)
            {
                IGridCellModelFactory pGridCellModelFactory = GridFactoryProvider.CellModelFactory;

                if (pGridCellModelFactory != null)
                {
                    e.CellModel = pGridCellModelFactory.CreateCellModel(e.CellType, Model);
                }
            }
        }

        /// <copyfrom cref="GridModel.CellModelsChanged"/>
        /// <summary>See <see cref="GridModel.CellModelsChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs when the CellModels collection is changed"),
        Category("Grid")]
        public event CollectionChangeEventHandler CellModelsChanged;

        /// <summary>
        /// Raises the <see cref="GridControl.CellModelsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CollectionChangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellModelsChanged(CollectionChangeEventArgs e)
        {
            if (CellModelsChanged != null)
            {
                CellModelsChanged(this, e);
            }
        }

         /// <summary>
        /// Raises the <see cref="GridControl.PrepareChangeSelection"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridPrepareChangeSelectionEventArgs" /> that contains the event data.</param>
        protected virtual void OnPrepareChangeSelection(GridPrepareChangeSelectionEventArgs e)
        {
            if (PrepareChangeSelection != null)
            {
                PrepareChangeSelection(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.CoveredRangesChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCoveredRangesChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnCoveredRangesChanging(GridCoveredRangesChangingEventArgs e)
        {
            if (CoveredRangesChanging != null)
            {
                CoveredRangesChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.CoveredRangesChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCoveredRangesChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnCoveredRangesChanged(GridCoveredRangesChangedEventArgs e)
        {
            if (CoveredRangesChanged != null)
            {
                CoveredRangesChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.BanneredRangesChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridBanneredRangesChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnBanneredRangesChanging(GridBanneredRangesChangingEventArgs e)
        {
            if (BanneredRangesChanging != null)
            {
                BanneredRangesChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.BanneredRangesChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridBanneredRangesChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnBanneredRangesChanged(GridBanneredRangesChangedEventArgs e)
        {
            if (BanneredRangesChanged != null)
            {
                BanneredRangesChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.FloatingCellsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridFloatingCellsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnFloatingCellsChanged(GridFloatingCellsChangedEventArgs e)
        {
            if (FloatingCellsChanged != null)
            {
                FloatingCellsChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControl.MergeCellsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridMergeCellsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnMergeCellsChanged(GridMergeCellsChangedEventArgs e)
        {
            if (MergeCellsChanged != null)
            {
                MergeCellsChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryCanMergeCells"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCanMergeCellsEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCanMergeCells(GridQueryCanMergeCellsEventArgs e)
        {
            if (QueryCanMergeCells != null)
            {
                QueryCanMergeCells(this, e);
            }
        }

        #endregion

        // from Model

        /// <copyfrom cref="GridModelOptions.ControllerOptionsChanged"/><summary>See <see cref="GridModelOptions.ControllerOptionsChanged"/> in the GridModel class for information.</summary>
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridControllerOptions.All)]
        [Description("Specifies which mouse controllers should be enabled for the grid.")]
        [RefreshProperties(RefreshProperties.All)]
        [Category("Grid")]
        public GridControllerOptions ControllerOptions
        {
            get
            {
                return Model.Options.ControllerOptions;
            }

            set
            {
                Model.Options.ControllerOptions = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.DataObjectConsumerOptions"/><summary>See <see cref="GridModelOptions.DataObjectConsumerOptions"/> in the GridModel class for information.</summary>
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridDataObjectConsumerOptions.All)]
        [Category("Grid")]
        [Description("Controls clipboard interchange format. Can be plain text and/or fully formatted with styles.")]
        public GridDataObjectConsumerOptions DataObjectConsumerOptions
        {
            get
            {
                return Model.Options.DataObjectConsumerOptions;
            }

            set
            {
                Model.Options.DataObjectConsumerOptions = value;
            }
        }

        /// <copyfrom cref="GridModel.ActiveGridView"/><summary>See <see cref="GridModel.ActiveGridView"/> in the GridModel class for information.</summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridControlBase ActiveGridView
        {
            get
            {
                return Model.ActiveGridView;
            }

            set
            {
                Model.ActiveGridView = value;
            }
        }

        /// <copyfrom cref="GridModel.UserData"/><summary>See <see cref="GridModel.UserData"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDictionary UserData
        {
            get
            {
                return Model.UserData;
            }
        }

        /// <copyfrom cref="GridModel.UpdateOptions"/><summary>See <see cref="GridModel.UpdateOptions"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override BeginUpdateOptions UpdateOptions
        {
            get
            {
                return Model.UpdateOptions;
            }
        }

        ////        ///// <copyfrom cref="GridModel.GridCellsRange"/><summary>See <see cref="GridModel.GridCellsRange"/> in the GridModel class for information.</summary>
        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public GridRangeInfo GridCellsRange 
        ////        {
        ////            get 
        ////            {
        ////                return Model.GridCellsRange;
        ////            }
        ////        }
        ////        ///// <copyfrom cref="GridModel.ScrollableGridRangeInfo"/><summary>See <see cref="GridModel.ScrollableGridRangeInfo"/> in the GridModel class for information.</summary>
        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public GridRangeInfo ScrollableGridRangeInfo 
        ////        {
        ////            get 
        ////            {
        ////                return Model.ScrollableGridRangeInfo;
        ////            }
        ////        }

        /// <copyfrom cref="GridModel.BaseStylesMap"/><summary>See <see cref="GridModel.BaseStylesMap"/> in the GridModel class for information.</summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The collection of base styles used in this grid.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid Contents")]
        public GridBaseStylesMap BaseStylesMap
        {
            get
            {
                return Model.BaseStylesMap;
            }

            set
            {
                Model.BaseStylesMap = value;
            }
        }

        bool ShouldSerializeBaseStylesMap()
        {
            return BaseStylesMap.Modified;
        }

        /// <summary>
        /// Resets the <see cref="BaseStylesMap"/> property.
        /// </summary>
        public void ResetBaseStylesMap()
        {
            BaseStylesMap.RegisterStandardStyles();

            GridModel model = Model;
            GridStyleInfo standard = model.BaseStylesMap["Standard"].StyleInfo;
            GridStyleInfo header = model.BaseStylesMap["Header"].StyleInfo;
            GridStyleInfo rowHeader = model.BaseStylesMap["Row Header"].StyleInfo;
            GridStyleInfo colHeader = model.BaseStylesMap["Column Header"].StyleInfo;

            GridFontInfo boldFont = new GridFontInfo();
            boldFont.Bold = true;
            boldFont.Size = 8;
            boldFont.Facename = "Verdana";
            ////standard.TextColor = Color.FromArgb(0, 21, 84);

            header.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
            rowHeader.Interior = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
            ////            header.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(51, 51, 102), Color.FromArgb(237, 240, 247));
            ////            rowHeader.Interior = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(51, 51, 102), Color.FromArgb(237, 240, 247));
            ////standard.Font.Size = 9;
            standard.Font.Facename = "Tahoma";
            //// standard.Interior = new BrushInfo(Color.FromArgb(237, 240, 247));

            BaseStylesMap.Modified = false;

            Refresh();
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="GridCellInfo"/> objects. This collection is a wrapper collection
        /// for cells in the <see cref="GridData"/> object. It provides support for code serialization at design-time.
        /// </summary>
        [Category("Grid Contents")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(false)]
        public GridCellInfoCollection GridCells
        {
            get
            {
                Model.GridCells.control = this;
                return Model.GridCells;
            }

            set
            {
                Model.GridCells = value;
                Model.GridCells.control = this;
            }
        }

        /// <summary>
        /// True if <see cref="SerializeCellsBehavior"/> is set to SerializeIntoCode, otherwise false.
        /// </summary>
        /// <returns>returns True if <see cref="SerializeCellsBehavior"/> is set to SerializeIntoCode, otherwise false.</returns>
        public bool ShouldSerializeGridCells()
        {
            return this.SerializeCellsBehavior == GridSerializeCellsBehavior.SerializeIntoCode;
        }

        /// <summary>
        /// Clears all cell formatting.   
        /// </summary>
        public void ResetGridCells()
        {
            Model.GridCells.Clear();
            ////Model.GridCells.WriteToData();
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="GridRangeStyle"/> objects. This collection is a wrapper collection
        /// for cells in the <see cref="GridData"/> object. It provides support for modifying
        /// cells through a CollectionEditor and code serialization at design-time.
        /// </summary>
        [Category("Grid Contents")]
        [Description("Defines the collection of styles for certain ranges.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridRangeStyleCollection RangeStyles
        {
            get
            {
                Model.RangeStyles.control = this;
                return Model.RangeStyles;
            }

            set
            {
                Model.RangeStyles = value;
                if (value != null)
                {
                    value.control = this;
                }
            }
        }

        /// <summary>
        /// Clears all cell formatting in the  <see cref="GridData"/> object.
        /// </summary>
        public void ResetRangeStyles()
        {
            Model.RangeStyles.Clear();
            Model.RangeStyles.WriteToData();
        }

        /// <summary>
        /// Determines if the range styles should be serialized.
        /// </summary>
        /// <returns>True if they should be serialized; False otherwise.</returns>
        public bool ShouldSerializeRangeStyles()
        {
            return this.SerializeCellsBehavior == GridSerializeCellsBehavior.SerializeAsRangeStylesIntoCode;
        }
        
        /// <summary>
        /// Gets or sets a collection of <see cref="GridRowHeight"/> objects. This collection is a wrapper collection
        /// for values in the <see cref="RowHeights"/> object. It provides support for modifying
        /// values through a CollectionEditor and code serialization at design-time.
        /// </summary>
        [Category("Grid Contents")]
        [Description("Set row heights interactively.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridRowHeightCollection RowHeightEntries
        {
            get
            {
                return Model.RowHeightEntries;
            }

            set
            {
                Model.RowHeightEntries = value;
            }
        }

        /// <summary>
        /// Determines if values in the <see cref="RowHeights"/> collection were modified.
        /// </summary>
        /// <returns>True if modified; False otherwise.</returns>
        public bool ShouldSerializeRowHeightEntries()
        {
            return RowHeights.Modified;
        }

        /// <summary>
        /// Resets values in the <see cref="RowHeights"/> collection.
        /// </summary>
        public void ResetRowHeightEntries()
        {
            GridRowColSizeDictionary rowColSizeDictionary = RowHeights.Dictionary as GridRowColSizeDictionary;
            if (rowColSizeDictionary != null)
            {
                GridIndexDictionary dict = rowColSizeDictionary.InnerDict;
                this.BeginUpdate(BeginUpdateOptions.None);
                dict.Clear();
                this.RowHeights[0] = 25;
                this.RowHeights.ResetModified();
                Model.rowHeightEntries = null;
                this.EndUpdate(false);
                this.Refresh();
            }
        }
        
        /// <summary>
        /// Gets or sets a collection of <see cref="GridColWidth"/> objects. This collection is a wrapper collection
        /// for values in the <see cref="ColWidths"/> object. It provides support for modifying
        /// values through a CollectionEditor and code serialization at design-time.
        /// </summary>
        [Category("Grid Contents")]
        [Description("Set columns widths interactively.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridColWidthCollection ColWidthEntries
        {
            get
            {
                return Model.ColWidthEntries;
            }

            set
            {
                Model.ColWidthEntries = value;
            }
        }

        /// <summary>
        /// Determines if values in the <see cref="ColWidths"/> collection were modified.
        /// </summary>
        /// <returns>True if modified; False otherwise.</returns>
        public bool ShouldSerializeColWidthEntries()
        {
            return ColWidths.Modified;
        }

        /// <summary>
        /// Resets values in the <see cref="ColWidths"/> collection.
        /// </summary>
        public void ResetColWidthEntries()
        {
            GridRowColSizeDictionary rowColSizeDictionary = ColWidths.Dictionary as GridRowColSizeDictionary;
            if (rowColSizeDictionary != null)
            {
                GridIndexDictionary dict = rowColSizeDictionary.InnerDict;
                this.BeginUpdate(BeginUpdateOptions.None);
                dict.Clear();
                this.ColWidths[0] = 35;
                this.ColWidths.ResetModified();
                Model.colWidthEntries = null;
                this.EndUpdate(false);
                this.Refresh();
            }
        }

        /// <copyfrom cref="GridModel.Data"/><summary>See <see cref="GridModel.Data"/> in the GridModel class for information.</summary>
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public GridData Data
        {
            get
            {
                return Model.Data;
            }

            set
            {
                if (value != null)
                {
                    Model.Data = value;
                }
            }
        }

        /// <copyfrom cref="GridModel.HasData"/><summary>See <see cref="GridModel.HasData"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasData
        {
            get
            {
                return Model.HasData;
            }
        }

        bool ShouldSerializeData()
        {
            return this.SerializeCellsBehavior == GridSerializeCellsBehavior.SerializeIntoResX;
        }

        /// <copyfrom cref="GridModel.IsUntitled "/><summary>See <see cref="GridModel.IsUntitled "/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsUntitled
        {
            get
            {
                return Model.IsUntitled;
            }
        }

        /// <copyfrom cref="GridModel.Modified "/><summary>See <see cref="GridModel.Modified "/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Modified
        {
            get
            {
                return Model.Modified;
            }

            set
            {
                Model.Modified = value;
            }
        }

        /// <copyfrom cref="GridModel.FileName "/><summary>See <see cref="GridModel.FileName "/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName
        {
            get
            {
                return Model.FileName;
            }

            set
            {
                Model.FileName = value;
            }
        }

        /// <copyfrom cref="GridModel.RowHeights "/><summary>See <see cref="GridModel.RowHeights "/> in the GridModel class for information.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [Category("Grid Contents")]
        [Description("Set row heights interactively.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        [Editor(typeof(RowHeightUITypeEditor), typeof(UITypeEditor))]
        public GridModelRowColSizeIndexer RowHeights
        {
            get
            {
                return Model.RowHeights;
            }

            set
            {
                if (value != null)
                {
                    value.SetModelInt(this.Model);
                    Model.SetRowHeightsInt(value);
                    Model.Refresh();
                }
            }
        }

        bool ShouldSerializeRowHeights()
        {
            return Model.RowHeights.Modified;
        }

        void ResetRowHeights()
        {
            if (Model.RowCount > 0)
            {
                Model.RowHeights.ResetRange(1, Model.RowCount);
            }
        }

        /// <copyfrom cref="GridModel.ColWidths "/><summary>See <see cref="GridModel.ColWidths "/> in the GridModel class for information.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [Category("Grid Contents")]
        [Description("Set columns widths interactively.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        [Editor(typeof(ColWidthUITypeEditor), typeof(UITypeEditor))]
        public GridModelRowColSizeIndexer ColWidths
        {
            get
            {
                return Model.ColWidths;
            }

            set
            {
                if (value != null)
                {
                    value.SetModelInt(this.Model);
                    Model.SetColWidthsInt(value);
                    Model.Refresh();
                }
            }
        }

        bool ShouldSerializeColWidths()
        {
            return Model.ColWidths.Modified;
        }

        void ResetColWidths()
        {
            if (Model.ColCount > 0)
            {
                Model.ColWidths.ResetRange(1, Model.ColCount);
            }
        }

        /// <copyfrom cref="GridModel.HideRows "/><summary>See <see cref="GridModel.HideRows "/> in the GridModel class for information.</summary>
        [Category("Grid Contents")]
        [Description("Hide rows interactively.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        [Editor(typeof(RowHideUITypeEditor), typeof(UITypeEditor))]
        public GridModelHideRowColsIndexer HideRows
        {
            get
            {
                return Model.HideRows;
            }

            set
            {
                if (value != null)
                {
                    value.SetModelInt(this.Model);
                    Model.SetHideRowsInt(value);
                    Model.Refresh();
                }
            }
        }

        bool ShouldSerializeHideRows()
        {
            return Model.HideRows.Modified;
        }

        void ResetHideRows()
        {
            if (Model.RowCount > 0)
            {
                Model.HideRows.ResetRange(1, Model.RowCount);
            }
        }

        /// <copyfrom cref="GridModel.HideCols "/><summary>See <see cref="GridModel.HideCols "/> in the GridModel class for information.</summary>
        [Category("Grid Contents")]
        [Description("Hide columns interactively.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        [Editor(typeof(ColHideUITypeEditor), typeof(UITypeEditor))]
        public GridModelHideRowColsIndexer HideCols
        {
            get
            {
                return Model.HideCols;
            }

            set
            {
                if (value != null)
                {
                    value.SetModelInt(this.Model);
                    Model.SetHideColsInt(value);
                    Model.Refresh();
                }
            }
        }

        bool ShouldSerializeHideCols()
        {
            return Model.HideCols.Modified;
        }

        void ResetHideCols()
        {
            if (Model.ColCount > 0)
            {
                Model.HideCols.ResetRange(1, Model.ColCount);
            }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="GridRowHidden"/> objects. This collection is a wrapper collection
        /// for values in the <see cref="HideRows"/> object. It provides support for modifying
        /// values through a CollectionEditor and code serialization at design-time.
        /// </summary>
        [Category("Grid Contents")]
        [Description("Hide rows interactively.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridRowHiddenCollection RowHiddenEntries
        {
            get
            {
                return Model.RowHiddenEntries;
            }

            set
            {
                Model.RowHiddenEntries = value;
            }
        }

        /// <summary>
        /// Determines if values in the <see cref="HideRows"/> collection were modified.
        /// </summary>
        /// <returns>True if modified; False otherwise.</returns>
        public bool ShouldSerializeRowHiddenEntries()
        {
            return HideRows.Modified;
        }

        /// <summary>
        /// Resets values in the <see cref="HideRows"/> collection.
        /// </summary>
        public void ResetRowHiddenEntries()
        {
            Model.ResetRowHiddenEntries();
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="GridColHidden"/> objects. This collection is a wrapper collection
        /// for values in the <see cref="HideCols"/> object. It provides support for modifying
        /// values through a CollectionEditor and code serialization at design-time.
        /// </summary>
        [Category("Grid Contents")]
        [Description("Hide columns interactively.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridColHiddenCollection ColHiddenEntries
        {
            get
            {
                return Model.ColHiddenEntries;
            }

            set
            {
                Model.ColHiddenEntries = value;
            }
        }

        /// <summary>
        /// Determines if values in the <see cref="HideCols"/> collection were modified.
        /// </summary>
        /// <returns>true if modified; false otherwise.</returns>
        public bool ShouldSerializeColHiddenEntries()
        {
            return HideCols.Modified;
        }

        /// <summary>
        /// Resets values in the <see cref="HideCols"/> collection.
        /// </summary>
        public void ResetColHiddenEntries()
        {
            Model.ResetColHiddenEntries();
        }

        /// <copyfrom cref="GridModel.CommandStack "/><summary>See <see cref="GridModel.CommandStack "/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelCommandManager CommandStack
        {
            get
            {
                return Model.CommandStack;
            }
        }

        /// <copyfrom cref="GridModel.Rows "/><summary>See <see cref="GridModel.Rows "/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelRowColOperations Rows
        {
            get
            {
                return Model.Rows;
            }
        }

        /// <copyfrom cref="GridModel.Cols "/><summary>See <see cref="GridModel.Cols "/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelRowColOperations Cols
        {
            get
            {
                return Model.Cols;
            }
        }

        /// <copyfrom cref="GridModelOptions.DefaultGridBorderStyle "/><summary>See <see cref="GridModelOptions.DefaultGridBorderStyle"/> in the GridModel class for information.</summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Specifies the border style to be used as default for cell borders.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        [DefaultValue(GridBorderStyle.Dotted)]
        public GridBorderStyle DefaultGridBorderStyle
        {
            get
            {
                return Model.Options.DefaultGridBorderStyle;
            }

            set
            {
                Model.Options.DefaultGridBorderStyle = value;
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
        [Description("Controls the kind of textbox control that is created for TextBox cells. In general the original text box behaves better than the default richtext box with Hebrew and arabic languages")]
        [Category("Grid")]
        public bool UseRightToLeftCompatibleTextBox
        {
            get
            {
                return Model.Options.UseRightToLeftCompatibleTextBox;
            }

            set
            {
                Model.Options.UseRightToLeftCompatibleTextBox = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.HighlightFrozenLine "/><summary>See <see cref="GridModelOptions.HighlightFrozenLine"/> in the GridModel class for information.</summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Specifies the border style to be used as default for cell borders.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool HighlightFrozenLine
        {
            get
            {
                return Model.Options.HighlightFrozenLine;
            }
            
            set
            {
                Model.Options.HighlightFrozenLine = value;
            }
        }

        /// <copyfrom cref="GridModel.DiscardReadOnly "/><summary>See <see cref="GridModel.DiscardReadOnly "/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DiscardReadOnly
        {
            get
            {
                return Model.DiscardReadOnly;
            }

            set
            {
                Model.DiscardReadOnly = value;
            }
        }

        /// <copyfrom cref="GridModel.ReadOnly "/><summary>See <see cref="GridModel.ReadOnly "/> in the GridModel class for information.</summary>
        [Browsable(true), DefaultValue(false)]
        [Description("The Read-only state of the grid.")]
        [Category("Grid Contents")]
        public bool ReadOnly
        {
            get
            {
                return Model.ReadOnly;
            }

            set
            {
                Model.ReadOnly = value;
            }
        }
        /// <summary>
        /// Enable the Grid state to browse only, can't able to edit and update.
        /// </summary>
        [Browsable(true), DefaultValue(false)]
        [Description("To specify the browse only state of the Grid.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Grid Contents")]
        public bool BrowseOnly
        {
            get
            {
                return Model.BrowseOnly;
            }

            set
            {
                Model.BrowseOnly = value;
            }
        }

        /// <copyfrom cref="GridModel.this[int,int]"/><summary>See <see cref="GridModel.this[int,int]"/> in the GridModel class for information.</summary>
        ////   /// <copyfrom cref="GridModel.Item(System.Int32, System.Int32)"/><summary>See <see cref="GridModel.Item(System.Int32, System.Int32)"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridStyleInfo this[int rowIndex, int colIndex]
        {
            get
            {
                return Model[rowIndex, colIndex];
            }

            set
            {
                Model[rowIndex, colIndex] = value;
            }
        }

        /// <copyfrom cref="GridModel.ColStyles"/><summary>See <see cref="GridModel.ColStyles"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Styles used by the columns in the grid. Column cells will inherit this style.")]
        [Category("Grid Contents")]
        public GridModelColStylesIndexer ColStyles
        {
            get
            {
                return Model.ColStyles;
            }

            set
            {
            }
        }

        /// <copyfrom cref="GridModel.RowStyles"/><summary>See <see cref="GridModel.RowStyles"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Styles used by the rows in the grid. Row cells will inherit this style.")]
        [Category("Grid Contents")]
        public GridModelRowStylesIndexer RowStyles
        {
            get
            {
                return Model.RowStyles;
            }
        }

        /// <copyfrom cref="GridModel.TableStyle"/><summary>See <see cref="GridModel.TableStyle"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Individual cells in the grid will inherit attributes from the table style.")]
        [Category("Grid Contents")]
        [RefreshProperties(RefreshProperties.All)]
        public GridStyleInfo TableStyle
        {
            get
            {
                return Model.TableStyle;
            }

            set
            {
                Model.TableStyle = value;
            }
        }

        bool ShouldSerializeTableStyle()
        {
            return !TableStyle.IsEmpty;
        }

        /// <summary>
        /// Resets the <see cref="TableStyle"/> property.
        /// </summary>
        public void ResetTableStyle()
        {
            TableStyle = new GridStyleInfo();
            Refresh();
        }

        /// <copyfrom cref="GridModel.CurrentCellInfo"/><summary>See <see cref="GridModel.CurrentCellInfo"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCurrentCellInfo CurrentCellInfo
        {
            get
            {
                return Model.CurrentCellInfo;
            }

            set
            {
                Model.CurrentCellInfo = value;
            }
        }

        /// <copyfrom cref="GridModel.HasCurrentCellInfo"/><summary>See <see cref="GridModel.HasCurrentCellInfo"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrentCellInfo
        {
            get
            {
                return Model.HasCurrentCellInfo;
            }
        }

        /// <copyfrom cref="GridModel.CurrentCellRenderer"/><summary>See <see cref="GridModel.CurrentCellRenderer"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCellRendererBase CurrentCellRenderer
        {
            get
            {
                return Model.CurrentCellRenderer;
            }
        }

        /// <copyfrom cref="GridModel.CutPaste"/><summary>See <see cref="GridModel.CutPaste"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelCutPaste CutPaste
        {
            get
            {
                return Model.CutPaste;
            }
        }

        /// <copyfrom cref="GridModel.DataExchange"/><summary>See <see cref="GridModel.DataExchange"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelStyleDataExchange DataExchange
        {
            get
            {
                return Model.DataExchange;
            }
        }

        /// <copyfrom cref="GridModel.TextDataExchange"/><summary>See <see cref="GridModel.TextDataExchange"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelTextDataExchange TextDataExchange
        {
            get
            {
                return Model.TextDataExchange;
            }
        }

        /// <copyfrom cref="GridModel.RowCount"/><summary>See <see cref="GridModel.RowCount"/> in the GridModel class for information.</summary>
        [Browsable(true), DefaultValue(10)]
        [Description("The number of rows in the grid.")]
        [Category("Grid Contents")]
        public int RowCount
        {
            get
            {
                return Model.RowCount;
            }

            set
            {
                Model.RowCount = value;
            }
        }

        /// <copyfrom cref="GridModel.ColCount"/><summary>See <see cref="GridModel.ColCount"/> in the GridModel class for information.</summary>
        [Browsable(true), DefaultValue(10)]
        [Description("The number of columns in the grid.")]
        [Category("Grid Contents")]
        public int ColCount
        {
            get
            {
                return Model.ColCount;
            }

            set
            {
                Model.ColCount = value;
            }
        }

        /// <copyfrom cref="GridModel.IgnoreReadOnly"/><summary>See <see cref="GridModel.IgnoreReadOnly"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IgnoreReadOnly
        {
            get
            {              
                return Model.IgnoreReadOnly;
            }

            set
            {
                Model.IgnoreReadOnly = value;
            }
        }

        ////        ///// <copyfrom cref="GridModel.Selections"/><summary>See <see cref="GridModel.Selections"/> in the GridModel class for information.</summary>
        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public GridModelSelections Selections
        ////        {
        ////            get
        ////            {
        ////                return Model.Selections;
        ////            }
        ////        }
        ////        internal GridRangeInfoList SelectedRanges 
        ////        {
        ////            get 
        ////            {
        ////                return Model.SelectedRanges;
        ////            }
        ////        }

        /// <copyfrom cref="GridModel.CoveredRanges"/><summary>See <see cref="GridModel.CoveredRanges"/> in the GridModel class for information.</summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The collection with covered ranges.")]
        [Category("Grid Contents")]
        public GridModelCoveredRanges CoveredRanges
        {
            get
            {
                return Model.CoveredRanges;
            }
        }

        /// <summary>
        /// Determines if covered ranges have been added. 
        /// </summary>
        /// <returns>returns boolean value if covered ranges have been added.</returns>
        bool ShouldSerializeCoveredRanges()
        {
            return Model.CoveredRanges.Count > 0;
        }

        /// <summary>
        /// Clears all covered ranges.
        /// </summary>
        public void ResetCoveredRanges()
        {
            if (Model.CoveredRanges.Count > 0)
            {
                Model.CoveredRanges.Clear();
            }
        }

        /// <copyfrom cref="GridModel.BanneredRanges"/><summary>See <see cref="GridModel.BanneredRanges"/> in the GridModel class for information.</summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The collection with banner ranges.")]
        [Category("Grid Contents")]
        public GridModelBanneredRanges BanneredRanges
        {
            get
            {
                return Model.BanneredRanges;
            }
        }

        /// <summary>
        /// Determines if bannered ranges have been added. 
        /// </summary>
        /// <returns>returns boolean value if bannered ranges have been added</returns>
        bool ShouldSerializeBanneredRanges()
        {
            return Model.BanneredRanges.Count > 0;
        }

        /// <summary>
        /// Clears all bannered ranges.
        /// </summary>
        public void ResetBanneredRanges()
        {
            if (Model.BanneredRanges.Count > 0)
            {
                Model.BanneredRanges.Clear();
            }
        }

        /// <copyfrom cref="GridModel.FloatingCells"/><summary>See <see cref="GridModel.FloatingCells"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelFloatingCells FloatingCells
        {
            get
            {
                return Model.FloatingCells;
            }
        }

        /// <copyfrom cref="GridModelOptions.FloatCellsMode"/><summary>See <see cref="GridModelOptions.FloatCellsMode"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(GridFloatCellsMode.None)]
        [Description("Enables and specifies floating cells behavior for the grid.")]
        [Category("Grid")]
        public GridFloatCellsMode FloatCellsMode
        {
            get
            {
                return Model.Options.FloatCellsMode;
            }

            set
            {
                Model.Options.FloatCellsMode = value;
                if (DesignMode)
                {
                    Refresh();
                }
            }
        }

        /// <copyfrom cref="GridModel.CellModels"/><summary>See <see cref="GridModel.CellModels"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCellModelCollection CellModels
        {
            get
            {
                return Model.CellModels;
            }
        }

        /// <copyfrom cref="GridModelOptions.ResizeRowsBehavior"/><summary>See <see cref="GridModelOptions.ResizeRowsBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.OutlineBounds)]
        [Description("Defines behavior for resizing rows.")]
        [Category("Grid")]
        public GridResizeCellsBehavior ResizeRowsBehavior
        {
            get
            {
                return Model.Options.ResizeRowsBehavior;
            }

            set
            {
                Model.Options.ResizeRowsBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ResizeColsBehavior"/><summary>See <see cref="GridModelOptions.ResizeColsBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.OutlineBounds)]
        [Description("Defines behavior for resizing columns.")]
        [Category("Grid")]
        public GridResizeCellsBehavior ResizeColsBehavior
        {
            get
            {
                return Model.Options.ResizeColsBehavior;
            }

            set
            {
                Model.Options.ResizeColsBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.AllowDragSelectedCols"/><summary>See <see cref="GridModelOptions.AllowDragSelectedCols"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(false)]
        [Description("Allow the user to drag selected columns by clicking on the column header.")]
        [Category("Grid")]
        public bool AllowDragSelectedCols
        {
            get
            {
                return Model.Options.AllowDragSelectedCols;
            }

            set
            {
                Model.Options.AllowDragSelectedCols = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.AllowDragSelectedRows"/><summary>See <see cref="GridModelOptions.AllowDragSelectedRows"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(false)]
        [Description("Controls allowing the user to drag selected rows by clicking on the row header.")]
        [Category("Grid")]
        public bool AllowDragSelectedRows
        {
            get
            {
                return Model.Options.AllowDragSelectedRows;
            }

            set
            {
                Model.Options.AllowDragSelectedRows = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.AlphaBlendSelectionColor"/><summary>See <see cref="GridModelOptions.AlphaBlendSelectionColor"/> in the GridModel class for information.</summary>
        [Browsable(true)]
        [Description("Specifies the color for alpha blended cell selections.")]
        [Category("Grid")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Color AlphaBlendSelectionColor
        {
            get
            {
                return Model.Options.AlphaBlendSelectionColor;
            }

            set
            {
                Model.Options.AlphaBlendSelectionColor = value;
            }
        }

        /// <summary>
        /// Resets the <see cref="AlphaBlendSelectionColor"/> property.
        /// </summary>
        public void ResetAlphaBlendSelectionColor()
        {
            Model.Options.AlphaBlendSelectionColor = SystemColors.Highlight;
        }

        bool ShouldSerializeAlphaBlendSelectionColor()
        {
            return Model.Options.AlphaBlendSelectionColor != Color.FromArgb(64, SystemColors.Highlight)
                && (Model.Options.AllowSelection & GridSelectionFlags.AlphaBlend) != 0;
        }

        /// <copyfrom cref="GridModelOptions.AllowSelection"/><summary>See <see cref="GridModelOptions.AllowSelection"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridSelectionFlags.Any)]
        [Description("Defines selection behavior of the grid.")]
        [Category("Grid")]
        public GridSelectionFlags AllowSelection
        {
            get
            {
                return Model.Options.AllowSelection;
            }

            set
            {
                Model.Options.AllowSelection = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.AllowScrollCurrentCellInView"/><summary>See <see cref="GridModelOptions.AllowScrollCurrentCellInView"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridScrollCurrentCellReason.Any)]
        [Description("Defines scroll cell in view behavior of the grid.")]
        [Category("Scrolling")]
        public GridScrollCurrentCellReason AllowScrollCurrentCellInView
        {
            get
            {
                return Model.Options.AllowScrollCurrentCellInView;
            }

            set
            {
                Model.Options.AllowScrollCurrentCellInView = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.DragSelectedCellsMouseButtonsMask"/><summary>See <see cref="GridModelOptions.SelectCellsMouseButtonsMask"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(MouseButtons.Left)]
        [Description("Defines which mouse buttons can be used for dragging selected rows or columns.")]
        [Category("Grid")]
        public MouseButtons DragSelectedCellsMouseButtonsMask
        {
            get
            {
                return Model.Options.DragSelectedCellsMouseButtonsMask;
            }

            set
            {
                Model.Options.DragSelectedCellsMouseButtonsMask = value;
            }
        }
        
        /// <copyfrom cref="GridModelOptions.SelectCellsMouseButtonsMask"/><summary>See <see cref="GridModelOptions.SelectCellsMouseButtonsMask"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right)]
        [Description("Defines which mouse buttons can be used for selecting cells.")]
        [Category("Grid")]
        public MouseButtons SelectCellsMouseButtonsMask
        {
            get
            {
                return Model.Options.SelectCellsMouseButtonsMask;
            }

            set
            {
                Model.Options.SelectCellsMouseButtonsMask = value;
            }
        }

        /// <summary>
        ///   <para> Gets or sets the method in which items are selected in
        /// the <see cref="GridControl" /> when it is being used in listbox mode
        /// .</para>
        /// </summary>
        [Category(@"Grid"),
        DefaultValue(SelectionMode.None),
        Description(@"Grid can simulate list boxes. In such mode indicates if the list box is to be single-select, multi-select, or unselectable.")]
        public virtual SelectionMode ListBoxSelectionMode
        {
            get
            {
                return Model.Options.ListBoxSelectionMode;
            }

            set
            {
                Model.Options.ListBoxSelectionMode = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ExcelLikeCurrentCell"/><summary>See <see cref="GridModelOptions.ExcelLikeCurrentCell"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(false)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Defines Excel-like current cell behavior. When the user moves the current cell out of a selected range, the range will be cleared.")]
        [Category("Grid")]
        public bool ExcelLikeCurrentCell
        {
            get
            {
                return Model.Options.ExcelLikeCurrentCell;
            }

            set
            {
                Model.Options.ExcelLikeCurrentCell = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ClickedOnDisabledCellBehavior"/>
        /// <summary>
        /// Gets or sets Excel-like current cell behavior. When the user clicks on a cell out of a selected range for which .Enabled has been set to false.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridClickedOnDisabledCellBehavior.Default)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Defines Excel-like current cell behavior. When the user clicks on a cell out of a selected range for which .Enabled has been set to false.")]
        [Category("Grid")]
        public GridClickedOnDisabledCellBehavior ClickedOnDisabledCellBehavior
        {
            get
            {
                return Model.Options.ClickedOnDisabledCellBehavior;
            }

            set
            {
                Model.Options.ClickedOnDisabledCellBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ExcelLikeSelectionFrame"/><summary>See <see cref="GridModelOptions.ExcelLikeSelectionFrame"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(false)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Specifies whether the active selection should be outlined with a selection frame.")]
        [Category("Grid")]
        public bool ExcelLikeSelectionFrame
        {
            get
            {
                return Model.Options.ExcelLikeSelectionFrame;
            }

            set
            {
                Model.Options.ExcelLikeSelectionFrame = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ActivateCurrentCellBehavior"/><summary>See <see cref="GridModelOptions.ActivateCurrentCellBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(GridCellActivateAction.ClickOnCell)]
        [Description("Specifies current cell activation behavior when moving the current cell or clicking inside a cell.")]
        [Category("Grid")]
        public GridCellActivateAction ActivateCurrentCellBehavior
        {
            get
            {
                return Model.Options.ActivateCurrentCellBehavior;
            }

            set
            {
                Model.Options.ActivateCurrentCellBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.EnterKeyBehavior"/><summary>See <see cref="GridModelOptions.EnterKeyBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(GridDirectionType.Right)]
        [Description("Controls what the grid does with the 'Enter' key.")]
        [Category("Grid")]
        public GridDirectionType EnterKeyBehavior
        {
            get
            {
                return Model.Options.EnterKeyBehavior;
            }

            set
            {
                Model.Options.EnterKeyBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.TransparentBackground"/><summary>See <see cref="GridModelOptions.TransparentBackground"/> in the GridModel class for information.</summary>
        [Browsable(false),
        DefaultValue(false)]
        [Description("Defines whether grid should erase and fill background of cells or only draw cell text.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool TransparentBackground
        {
            get
            {
                return Model.Options.TransparentBackground;
            }

            set
            {
                Model.Options.TransparentBackground = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.NumberedRowHeaders"/><summary>See <see cref="GridModelOptions.NumberedRowHeaders"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(true)]
        [Description("Toggle display of row numbers in row headers.")]
        [Category("Grid")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool NumberedRowHeaders
        {
            get
            {
                return Model.Options.NumberedRowHeaders;
            }

            set
            {
                Model.Options.NumberedRowHeaders = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.NumberedColHeaders"/><summary>See <see cref="GridModelOptions.NumberedColHeaders"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(true)]
        [Description("Toggle display of column ids (A, B, C, ...) in column headers.")]
        [Category("Grid")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool NumberedColHeaders
        {
            get
            {
                return Model.Options.NumberedColHeaders;
            }

            set
            {
                Model.Options.NumberedColHeaders = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should be display column headers.
        /// </summary>
        [Description("Specifies if column headers should be displayed or hidden.")]
        [Browsable(true), DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        public bool ShowColumnHeaders
        {
            get
            {
                return Properties.ColHeaders;
            }

            set
            {
                Properties.ColHeaders = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether horizontal lines should be displayed.
        /// </summary>
        [Description("Specifies if horizontal lines should be displayed.")]
        [Browsable(true), DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        public bool DisplayHorizontalLines
        {
            get
            {
                return Properties.DisplayHorzLines;
            }

            set
            {
                Properties.DisplayHorzLines = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether vertical lines should be displayed.
        /// </summary>
        [Description("Specifies if vertical lines should be displayed.")]
        [Browsable(true), DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        public bool DisplayVerticalLines
        {
            get
            {
                return Properties.DisplayVertLines;
            }

            set
            {
                Properties.DisplayVertLines = value;
            }
        }
        /// <summary>
        /// Gets or sets the color of grid lines.
        /// </summary>
        [Description("The color of grid lines.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        public Color GridLineColor
        {
            get
            {
                return Properties.GridLineColor;
            }

            set
            {
                Properties.GridLineColor = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether column headers should be printed when printing the grid.
        /// </summary>
        [Description("Specifies if column headers should be printed when printing the grid.")]
        [Category("Grid")]
        [Browsable(true), DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintColumnHeader
        {
            get
            {
                return Properties.PrintColHeader;
            }

            set
            {
                Properties.PrintColHeader = value;
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
                return Properties.PrintHorzLines;
            }

            set
            {
                Properties.PrintHorzLines = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether row headers should be printed when printing the grid.
        /// </summary>
        [Description("Specifies if row headers should be printed when printing the grid.")]
        [Browsable(true), DefaultValue(true)]
        [Category("Grid")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintRowHeader
        {
            get
            {
                return Properties.PrintRowHeader;
            }

            set
            {
                Properties.PrintRowHeader = value;
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
                return Properties.PrintVertLines;
            }

            set
            {
                Properties.PrintVertLines = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether row headers should be displayed or hidden. (Might be better to use HideCols[0] = false) instead.
        /// </summary>
        [Description("Specifies if row headers should be displayed or hidden.")]
        [Browsable(true), DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        public bool ShowRowHeaders
        {
            get
            {
                return Properties.RowHeaders;
            }

            set
            {
                Properties.RowHeaders = value;
            }
        }
        /// <copyfrom cref="GridModelOptions.AllowThumbTrack"/><summary>See <see cref="GridModelOptions.AllowThumbTrack"/> in the GridModel class for information.</summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Obsolete("Use HorizontalThumbTrack and VerticalThumbTrack properties instead.")]
        public bool AllowThumbTrack
        {
            get
            {
                return Model.Options.AllowThumbTrack;
            }

            set
            {
                Model.Options.AllowThumbTrack = SupportsThumbTrack = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.HorizontalThumbTrack"/><summary>See <see cref="GridModelOptions.HorizontalThumbTrack"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Category("Scrolling")]
        [Description("Defines whether the grid should scroll immediately when the user grabs a horizontal scrollbar thumb and drags it.")]
        public override bool HorizontalThumbTrack
        {
            get
            {
                return Model.Options.HorizontalThumbTrack;
            }

            set
            {
                base.HorizontalThumbTrack = Model.Options.HorizontalThumbTrack = base.HorizontalThumbTrack = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.VerticalThumbTrack"/><summary>See <see cref="GridModelOptions.VerticalThumbTrack"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Category("Scrolling")]
        [Description("Specifies if the control should scroll while the user is dragging a vertical scrollbar thumb.")]
        public override bool VerticalThumbTrack
        {
            get
            {
                return Model.Options.VerticalThumbTrack;
            }

            set
            {
                base.VerticalThumbTrack = Model.Options.VerticalThumbTrack = base.VerticalThumbTrack = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.HorizontalScrollTips"/><summary>See <see cref="GridModelOptions.HorizontalScrollTips"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Category("Scrolling")]
        [Description("Defines whether the grid should display Scroll Tips when the user grabs a horizontal scrollbar thumb and drags it.")]
        public override bool HorizontalScrollTips
        {
            get
            {
                return Model.Options.HorizontalScrollTips;
            }

            set
            {
                base.HorizontalScrollTips = Model.Options.HorizontalScrollTips = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.VerticalScrollTips"/><summary>See <see cref="GridModelOptions.VerticalScrollTips"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Category("Scrolling")]
        [Description("Defines whether the grid should display Scroll Tips when the user grabs a vertical scrollbar thumb and drags it.")]
        public override bool VerticalScrollTips
        {
            get
            {
                return Model.Options.VerticalScrollTips;
            }

            set
            {
                base.VerticalScrollTips = Model.Options.VerticalScrollTips = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.MinResizeRowSize"/><summary>See <see cref="GridModelOptions.MinResizeRowSize"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(0)]
        [Description("Defines the minimum row height when the user resizes a row with the mouse.")]
        [Category("Grid")]
        public int MinResizeRowSize
        {
            get
            {
                return Model.Options.MinResizeRowSize;
            }

            set
            {
                Model.Options.MinResizeRowSize = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.MinResizeColSize"/><summary>See <see cref="GridModelOptions.MinResizeColSize"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(0)]
        [Description("Defines the minimum column width when the user resizes a column with the mouse.")]
        [Category("Grid")]
        public int MinResizeColSize
        {
            get
            {
                return Model.Options.MinResizeColSize;
            }

            set
            {
                Model.Options.MinResizeColSize = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.SmoothControlResize"/><summary>See <see cref="GridModelOptions.SmoothControlResize"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(true)]
        [Description("Defines whether a grid should be completely refreshed when the user resizes the window or if only newly visible rows or columns should be redrawn.")]
        [Category("Grid")]
        public bool SmoothControlResize
        {
            get
            {
                return Model.Options.SmoothControlResize;
            }

            set
            {
                Model.Options.SmoothControlResize = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ScrollFrozen"/><summary>See <see cref="GridModelOptions.ScrollFrozen"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(true)]
        [Description("Defines scroll behavior when user moves current cell with arrow keys into frozen cells area.")]
        [Category("Scrolling")]
        public bool ScrollFrozen
        {
            get
            {
                return Model.Options.ScrollFrozen;
            }

            set
            {
                Model.Options.ScrollFrozen = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.DrawOrder"/><summary>See <see cref="GridModelOptions.DrawOrder"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(GridDrawOrder.Rows)]
        [Description("Defines the order how cells are loaded before the grid is displayed.")]
        [Category("Grid")]
        public GridDrawOrder DrawOrder
        {
            get
            {
                return Model.Options.DrawOrder;
            }

            set
            {
                Model.Options.DrawOrder = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ShowCurrentCellBorderBehavior"/><summary>See <see cref="GridModelOptions.ShowCurrentCellBorderBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(GridShowCurrentCellBorder.WhenGridActive)]
        [Description("Defines when to show current cell frame or border.")]
        [Category("Grid")]
        public GridShowCurrentCellBorder ShowCurrentCellBorderBehavior
        {
            get
            {
                return Model.Options.ShowCurrentCellBorderBehavior;
            }

            set
            {
                Model.Options.ShowCurrentCellBorderBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.RefreshCurrentCellBehavior"/><summary>See <see cref="GridModelOptions.ShowCurrentCellBorderBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(GridRefreshCurrentCellBehavior.RefreshCell)]
        [Description("Which cells to refresh when moving the current cell.")]
        [Category("Grid")]
        public GridRefreshCurrentCellBehavior RefreshCurrentCellBehavior
        {
            get
            {
                return Model.Options.RefreshCurrentCellBehavior;
            }

            set
            {
                Model.Options.RefreshCurrentCellBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ShouldSynchronizeCurrentCell"/><summary>See <see cref="GridModelOptions.ShouldSynchronizeCurrentCell"/> in the GridModel class for information.</summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShouldSynchronizeCurrentCell
        {
            get
            {
                return Model.Options.ShouldSynchronizeCurrentCell;
            }

            set
            {
                Model.Options.ShouldSynchronizeCurrentCell = value;
            }
        }

        /// <copyfrom cref="GridModel.Properties"/><summary>See <see cref="GridModel.Properties"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Manages more options for the grid. Printing related. Also manages colors for grid background, grid lines, and more.")]
        [Category("Grid Contents")]
        public GridProperties Properties
        {
            get
            {
                return Model.Properties;
            }

            set
            {
                Model.Properties = value;
            }
        }

        /// <summary>
        /// Enable or Disable the Legacy styles in the Table Model
        /// Value should be false to apply ColorStyles
        /// </summary>
        [Description("Allow Legacy Styles to Enable or Disable")]
        [Browsable(true), DefaultValue(true)]
        [Category("Grid")]
        public bool ApplyVisualStyles
        {
            get
            {
                return Model.EnableLegacyStyle;
            }
            set
            {
                if (Model.EnableLegacyStyle != value)
                {
                    Model.EnableLegacyStyle = value;
                }
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
                this.Model.EnableLegacyStyle = false;
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
        /// Gets or sets the VisualStyles (skins) like Office2010, Office2007, Office2003
        /// </summary>
        [Browsable(true),
        DefaultValue(GridVisualStyles.SystemTheme)]
        [Description("Specifies look and feel skins for the Grid")]
        [Category("Grid")]
        public GridVisualStyles GridVisualStyles
        {
            get
            {
                return Model.Options.GridVisualStyles;
            }

            set
            {
                GridVisualStyles visualStyles = Model.Options.GridVisualStyles;
                Model.Options.GridVisualStyles = value;
                if (value == GridVisualStyles.Metro)
                {
                    if (!this.PersistAppearanceSettings)
                    {
                        this.DefaultRowHeight = 20;
                        GridStyleInfo.Default.BackColor = Color.White;
                        GridStyleInfo.Default.TextColor = Color.FromArgb(91, 91, 91);
                        GridStyleInfo.Default.Font.Facename = "Segoe UI";
                        GridStyleInfo.Default.Font.Size = 9F;
                        Model.Options.DefaultGridBorderStyle = GridBorderStyle.Solid;
                        Model.Properties.GridLineColor = Color.FromArgb(212, 212, 212);
                        GridStyleInfo.Default.Borders.Right = new GridBorder(Model.Options.DefaultGridBorderStyle, Model.Properties.GridLineColor, GridBorderWeight.ExtraThin);
                        this.RowHeights[0] = 29;
                        isMetroSettingsApplied = true;
                    }
                    this.GridOfficeScrollBars = OfficeScrollBars.Metro;
                }
                if (!this.Model.EnableLegacyStyle && value != GridVisualStyles.Metro)
                {
                    switch (value)
                    {
                        case GridVisualStyles.Office2007Blue:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
                            GridStyleInfo.Default.BackColor = Color.White;
                            GridStyleInfo.Default.TextColor = Color.DarkSlateGray;
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            break;

                        case GridVisualStyles.Office2007Black:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Black;
                            GridStyleInfo.Default.TextColor = SystemColors.InactiveCaptionText;
                            GridStyleInfo.Default.BackColor = Color.White;
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(205, 200, 177), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(205, 200, 177), GridBorderWeight.ExtraThin);
                            break;

                        case GridVisualStyles.Office2007Silver:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Silver;
                            GridStyleInfo.Default.BackColor = Color.FromArgb(252, 252, 252);
                            GridStyleInfo.Default.TextColor = Color.FromArgb(51, 51, 51);
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            break;

                        case GridVisualStyles.Office2010Blue:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
                            GridStyleInfo.Default.BackColor = Color.White;
                            GridStyleInfo.Default.TextColor = Color.DarkSlateGray;
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            break;

                        case GridVisualStyles.Office2010Black:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Black;
                            GridStyleInfo.Default.TextColor = SystemColors.InactiveCaptionText;
                            GridStyleInfo.Default.BackColor = Color.White;
                            this.BaseStylesMap.ColumnHeader.StyleInfo.TextColor = Color.White;
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(205, 200, 177), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(205, 200, 177), GridBorderWeight.ExtraThin);
                            break;

                        case GridVisualStyles.Office2010Silver:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Silver;
                            GridStyleInfo.Default.BackColor = Color.FromArgb(252, 252, 252);
                            GridStyleInfo.Default.TextColor = Color.FromArgb(51, 51, 51);
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            break;
                        default:
                            this.GridOfficeScrollBars = OfficeScrollBars.None;
                            GridStyleInfo.Default.BackColor = Color.White;
                            GridStyleInfo.Default.TextColor = Color.Black;
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Standard, SystemColors.Control);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Standard, SystemColors.Control);
                            break;
                    }
                    if (isMetroSettingsApplied)
                    {
                        GridStyleInfo.Default.Font.Size = 8.25f;
                        Model.Options.DefaultGridBorderStyle = GridBorderStyle.Dotted;
                        Model.Properties.GridLineColor = Color.FromArgb(109, 109, 109);
                        this.RowHeights[0] = 25;
                        this.DefaultRowHeight = 20;
                        this.BaseStylesMap.ColumnHeader.StyleInfo.HorizontalAlignment = GridHorizontalAlignment.Center;
                        isMetroSettingsApplied = false;
                    }
                }
                else if (isMetroSettingsApplied && value != GridVisualStyles.Metro)
                {
                    GridStyleInfo.Default.Font.Size = 8.25f;
                    GridStyleInfo.Default.TextColor = (value == GridVisualStyles.Office2010Black) ? Color.White : SystemColors.WindowText;
                    Model.Options.DefaultGridBorderStyle = GridBorderStyle.Dotted;
                    Model.Properties.GridLineColor = Color.FromArgb(109, 109, 109);
                    this.DefaultRowHeight = 17;
                    this.RowHeights[0] = 21;
                    if (Model.GetType().Name == "GridListControlModel")
                        GridStyleInfo.Default.TextColor = SystemColors.WindowText;
                    isMetroSettingsApplied = false;
                }
                if (Model.GetType().Name == "GridListControlModel" && value != GridVisualStyles.Metro && this.Model.EnableLegacyStyle)
                {
                    GridStyleInfo.Default.Borders.Bottom = GridBorder.Empty;
                }
                if (this.DpiAware)
                {
                    this.DefaultRowHeight = base.RowHeightOnScaling();
                    this.Model.ColWidths[0] = this.DefaultRowHeight + 8;//Header padding value is increased by 8 regardless of normal rows.
                }
            }
        }

        /// <summary>
        /// Sets the Custom metro colors to the Grid.
        /// </summary>
        /// <param name="metroColor">Custom Metro Color</param>
        /// <param name="metroHoverColor">Custom MouseHover Color</param>
        /// <param name="metroColorPressed">Custom PushButtonPress Color</param>
        public void SetMetroStyle(object metroColor, object metroHoverColor, object metroColorPressed)
        {
            this.Model.Options.SetMetroStyles(metroColor, metroHoverColor, metroColorPressed);
            this.GridVisualStyles = GridVisualStyles.Metro;
        }

        /// <summary>
        /// set the color for metro theme in Grid
        /// </summary>
        /// <param name="metroColors">Collection of metro colors</param>
        public void SetMetroStyle(GridMetroColors metroColors)
        {
            this.Model.Options.SetMetroStyles(metroColors);
            this.GridVisualStyles = GridVisualStyles.Metro;
        }
        /// <summary>
        /// [Deprecated]Gets or sets the VisualStyles (skins) like Office2010, Office2007, Office2003
        /// </summary>
        [Browsable(true),
        DefaultValue(ColorStyles.SystemTheme)]
        [Description("[Deprecated] Specifies look and feel skins for the Grid")]
        [Category("Grid")]
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
                    case ColorStyles.SystemTheme:
                        this.GridVisualStyles = GridVisualStyles.SystemTheme;
                        break;
                }
            }
        }
      

        /// <summary>
        /// Gets or sets the VisualStylesDrawing object
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the VisualStylesDrawing object")]
        [Category("Grid")]
        public IVisualStylesDrawing GridVisualStylesDrawing
        {
            get
            {
                return Model.Options.GridVisualStylesDrawing;
            }

            set
            {
                Model.Options.GridVisualStylesDrawing = value;
            }
        }
        
        bool ShouldSerializeProperties()
        {
            return Model.Properties.Modified;
        }

        /// <summary>
        /// Occurs when the <see cref="GridModel.QueryCellModel"/> is querying for the <see cref="GridCellModelBase"/>
        /// based on a string cellType.
        /// </summary>    
        /// <remarks>
        /// The GridModel has a table with all cell types used in the grid. Whenever the grid encounters
        /// a new cell type that it cannot find in the table, it will raise a <see cref="GridModel.QueryCellModel"/> event.
        /// The <see cref="GridStyleInfo.CellType"/> identifies the name of the cell type. The 
        /// <see cref="GridQueryCellModelEventArgs.CellModel"/> should receive the new instance of the
        /// associated cell object. This object will be stored in the table together with its name and
        /// reused among cells with the same <see cref="GridStyleInfo.CellType"/>.
        /// <para/>
        /// You should process this event if you want to add custom cell types and initialize these
        /// cell types on demand when associated cells are accessed the first time.
        /// </remarks>
        /// <seealso cref="GridQueryCellModelEventHandler"/>
        [Description("Occurs when GridModel.QueryCellModel is querying for the GridCellModelBase based on a string cellType")]
        [Category("Grid")]
        public event GridQueryCellModelEventHandler QueryCellModel;
        
        /// <copyfrom cref="GridModelOptions.ControllerOptionsChanged"/><summary>See <see cref="GridModelOptions.ControllerOptionsChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs when controller options have changed.")]
        [Category("Grid")]
        public event EventHandler ControllerOptionsChanged;

        /// <copyfrom cref="GridModelOptions.DataObjectConsumerOptionsChanged"/><summary>See <see cref="GridModelOptions.DataObjectConsumerOptionsChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs when DataObjectConsumerOptions have changed.")]
        [Category("Grid")]
        public event EventHandler DataObjectConsumerOptionsChanged;

        /// <copyfrom cref="GridModel.BaseStylesMapChanged"/><summary>See <see cref="GridModel.BaseStylesMapChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs when the reference to the BaseStylesMap has changed.")]
        [Category("Grid")]
        public event EventHandler BaseStylesMapChanged;

        /// <copyfrom cref="GridModel.DataChanged"/><summary>See <see cref="GridModel.DataChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs when the modified flag has changed."),
        Category("Grid")]
        public event EventHandler DataChanged;

        /// <copyfrom cref="GridModel.ModifiedChanged"/><summary>See <see cref="GridModel.ModifiedChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs when the Modified property has been changed.")]
        [Category("Grid")]
        public event EventHandler ModifiedChanged;

        /// <copyfrom cref="GridModel.FileNameChanged"/><summary>See <see cref="GridModel.FileNameChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs when FileName has changed."),
        Category("Grid")]
        public event EventHandler FileNameChanged;

        /// <copyfrom cref="GridModel.CellsChanged"/><summary>See <see cref="GridModel.CellsChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after the contents of a specified range of cells have changed."),
        Category("Grid")]
        public event GridCellsChangedEventHandler CellsChanged;

        /// <copyfrom cref="GridModel.CellsChanging"/><summary>See <see cref="GridModel.CellsChanging"/> in the GridModel class for information.</summary>
        [Description(" Occurs before the contents of a specified range of cells are being changed."),
        Category("Grid")]
        public event GridCellsChangingEventHandler CellsChanging;

        /// <copyfrom cref="GridModel.ConfirmingPendingChanges"/><summary>See <see cref="GridModel.ConfirmingPendingChanges"/> in the GridModel class for information.</summary>
        [Description("Occurs when any changes in the grid should be confirmed."),
        Category("Grid")]
        public event CancelEventHandler ConfirmingPendingChanges;

        /// <copyfrom cref="GridModel.RefreshRequest"/><summary>See <see cref="GridModel.RefreshRequest"/> in the GridModel class for information.</summary>
        [Description("Occurs when Refresh was called."),
        Category("Grid")]
        public event EventHandler RefreshRequest;

        /// <copyfrom cref="GridModel.BeginUpdateRequest"/><summary>See <see cref="GridModel.BeginUpdateRequest"/> in the GridModel class for information.</summary>
        [Description("Occurs when the first BeginUpdate was called for the grid model."),
        Category("Grid")]
        public event EventHandler BeginUpdateRequest;

        /// <copyfrom cref="GridModel.EndUpdateRequest"/><summary>See <see cref="GridModel.EndUpdateRequest"/> in the GridModel class for information.</summary>
        [Description("Occurs when the last EndUpdate was called for the grid model."),
        Category("Grid")]
        public event GridEndUpdateRequestEventHandler EndUpdateRequest;

        /// <copyfrom cref="GridModel.SaveCellInfo"/><summary>See <see cref="GridModel.SaveCellInfo"/> in the GridModel class for information.</summary>
        [Description("Occurs when the model is about to save style information about a specific cell."),
        Category("Grid")]
        public event GridSaveCellInfoEventHandler SaveCellInfo;

        /// <copyfrom cref="GridModel.PasteCellText"/><summary>See <see cref="GridModel.PasteCellText"/> in the GridModel class for information.</summary>
        [Description("Occurs when the model is about to save style information about a specific cell."),
        Category("Grid")]
        public event GridPasteCellTextEventHandler PasteCellText;

        /// <copyfrom cref="GridModel.QueryCellInfo"/><summary>See <see cref="GridModel.QueryCellInfo"/> in the GridModel class for information.</summary>
        [Description("Occurs when the model queries for style information about a specific cell."),
        Category("Grid")]
        public event GridQueryCellInfoEventHandler QueryCellInfo;

        /// <copyfrom cref="GridModel.QueryCoveredRange"/><summary>See <see cref="GridModel.QueryCoveredRange"/> in the GridModel class for information.</summary>
        [Description("Occurs when the model queries information about covered cells at a specific cell."),
        Category("Grid")]
        public event GridQueryCoveredRangeEventHandler QueryCoveredRange;

        /// <copyfrom cref="GridModel.QueryBanneredRange"/><summary>See <see cref="GridModel.QueryBanneredRange"/> in the GridModel class for information.</summary>
        [Description("Occurs when the model queries information about bannered cells at a specific cell."),
        Category("Grid")]
        public event GridQueryBanneredRangeEventHandler QueryBanneredRange;

        /// <copyfrom cref="GridModel.QueryColCount"/><summary>See <see cref="GridModel.QueryColCount"/> in the GridModel class for information.</summary>
        [Description("Occurs before the column count is returned from the model."),
        Category("Grid")]
        public event GridRowColCountEventHandler QueryColCount;

        /// <copyfrom cref="GridModel.QueryRowCount"/><summary>See <see cref="GridModel.QueryRowCount"/> in the GridModel class for information.</summary>
        [Description("Occurs before the row count is returned from the model."),
        Category("Grid")]
        public event GridRowColCountEventHandler QueryRowCount;

        /// <copyfrom cref="GridModel.SaveColCount"/><summary>See <see cref="GridModel.SaveColCount"/> in the GridModel class for information.</summary>
        [Description("Occurs before the column count is changed in the model."),
        Category("Grid")]
        public event GridRowColCountEventHandler SaveColCount;

        /// <copyfrom cref="GridModel.SaveRowCount"/><summary>See <see cref="GridModel.SaveRowCount"/> in the GridModel class for information.</summary>
        [Description("Occurs before the row count is changed in the model."),
        Category("Grid")]
        public event GridRowColCountEventHandler SaveRowCount;

        /// <copyfrom cref="GridModel.PrepareGraphics"/><summary>See <see cref="GridModel.PrepareGraphics"/> in the GridModel class for information.</summary>
        [Description("Occurs when an operation takes a longer time and the user should be notified about its status."),
        Category("Grid")]
        public event GraphicsEventHandler PrepareGraphics;

        /// <copyfrom cref="GridModel.OperationFeedback"/><summary>See <see cref="GridModel.OperationFeedback"/> in the GridModel class for information.</summary>
        [Description("Occurs when an operation takes a longer time and the user should be notified about its status."),
        Category("Grid")]
        public event OperationFeedbackEventHandler OperationFeedback;

        /// <copyfrom cref="GridModel.RowHeightsChanged"/><summary>See <see cref="GridModel.RowHeightsChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after row heights for a specified range of rows has changed."),
        Category("Grid")]
        public event GridRowColSizeChangedEventHandler RowHeightsChanged;

        /// <copyfrom cref="GridModel.RowHeightsChanging"/><summary>See <see cref="GridModel.RowHeightsChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before row heights for a specified range of rows are changed."),
        Category("Grid")]
        public event GridRowColSizeChangingEventHandler RowHeightsChanging;

        /// <copyfrom cref="GridModel.ColWidthsChanged"/><summary>See <see cref="GridModel.ColWidthsChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after column widths for a specified range of columns have been changed."),
        Category("Grid")]
        public event GridRowColSizeChangedEventHandler ColWidthsChanged;

        /// <copyfrom cref="GridModel.ColWidthsChanging"/><summary>See <see cref="GridModel.ColWidthsChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before column widths for a specified range of columns are changed."),
        Category("Grid")]
        public event GridRowColSizeChangingEventHandler ColWidthsChanging;

        /// <copyfrom cref="GridModel.QueryColWidth"/><summary>See <see cref="GridModel.QueryColWidth"/> in the GridModel class for information.</summary>
        [Description("Occurs before the size of a column is returned from the dictionary."),
        Category("Grid")]
        public event GridRowColSizeEventHandler QueryColWidth;

        /// <copyfrom cref="GridModel.QueryRowHeight"/><summary>See <see cref="GridModel.QueryRowHeight"/> in the GridModel class for information.</summary>
        [Description("Occurs before the size of a row is returned from the dictionary."),
        Category("Grid")]
        public event GridRowColSizeEventHandler QueryRowHeight;

        /// <copyfrom cref="GridModel.SaveColWidth"/><summary>See <see cref="GridModel.SaveColWidth"/> in the GridModel class for information.</summary>
        [Description("Occurs before the size of a column is stored in the dictionary."),
        Category("Grid")]
        public event GridRowColSizeEventHandler SaveColWidth;

        /// <copyfrom cref="GridModel.SaveRowHeight"/><summary>See <see cref="GridModel.SaveRowHeight"/> in the GridModel class for information.</summary>
        [Description("Occurs before the size of a row is stored in the dictionary."),
        Category("Grid")]
        public event GridRowColSizeEventHandler SaveRowHeight;

        /// <copyfrom cref="GridModel.ColsHidden"/><summary>See <see cref="GridModel.ColsHidden"/> in the GridModel class for information.</summary>
        [Description("Occurs after a range of columns was hidden."),
        Category("Grid")]
        public event GridRowColHiddenEventHandler ColsHidden;

        /// <copyfrom cref="GridModel.ColsHiding"/><summary>See <see cref="GridModel.ColsHiding"/> in the GridModel class for information.</summary>
        [Description("Occurs before a range of columns is hidden."),
        Category("Grid")]
        public event GridRowColHidingEventHandler ColsHiding;

        /// <copyfrom cref="GridModel.RowsHidden"/><summary>See <see cref="GridModel.RowsHidden"/> in the GridModel class for information.</summary>
        [Description("Occurs after a range of rows was hidden."),
        Category("Grid")]
        public event GridRowColHiddenEventHandler RowsHidden;

        /// <copyfrom cref="GridModel.RowsHiding"/><summary>See <see cref="GridModel.RowsHiding"/> in the GridModel class for information.</summary>
        [Description("Occurs before a range of rows is hidden."),
        Category("Grid")]
        public event GridRowColHidingEventHandler RowsHiding;

        /// <copyfrom cref="GridModel.QueryHideCol"/><summary>See <see cref="GridModel.QueryHideCol"/> in the GridModel class for information.</summary>
        [Description("Occurs before the hidden state of a column is returned from the dictionary."),
        Category("Grid")]
        internal event GridRowColHideEventHandler QueryHideCol; ////not currently used - so marked as internal

        /// <copyfrom cref="GridModel.SaveHideCol"/><summary>See <see cref="GridModel.SaveHideCol"/> in the GridModel class for information.</summary>
        [Description("Occurs before the hidden state of a column is stored in the dictionary."),
        Category("Grid")]
        internal event GridRowColHideEventHandler SaveHideCol;

        /// <copyfrom cref="GridModel.QueryHideRow"/><summary>See <see cref="GridModel.QueryHideRow"/> in the GridModel class for information.</summary>
        [Description("Occurs before the hidden state of a row is returned from the dictionary."),
        Category("Grid")]
        internal event GridRowColHideEventHandler QueryHideRow; ////not currently used - so marked as internal

        /// <copyfrom cref="GridModel.SaveHideRow"/><summary>See <see cref="GridModel.SaveHideRow"/> in the GridModel class for information.</summary>
        [Description("Occurs before the hidden state of a row is stored in the dictionary."),
        Category("Grid")]
        internal event GridRowColHideEventHandler SaveHideRow;

        /// <copyfrom cref="GridModel.DefaultRowHeightChanging"/><summary>See <see cref="GridModel.DefaultRowHeightChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before the default row height is changed."),
        Category("Grid")]
        public event GridDefaultSizeChangingEventHandler DefaultRowHeightChanging;

        /// <copyfrom cref="GridModel.DefaultRowHeightChanged"/><summary>See <see cref="GridModel.DefaultRowHeightChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after the default row height has been changed."),
        Category("Grid")]
        public event GridDefaultSizeChangedEventHandler DefaultRowHeightChanged;

        /// <copyfrom cref="GridModel.HeaderRowCountChanged"/><summary>See <see cref="GridModel.HeaderRowCountChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after the header row count has been changed."),
        Category("Grid")]
        public event GridCountChangedEventHandler HeaderRowCountChanged;

        /// <copyfrom cref="GridModel.HeaderRowCountChanging"/><summary>See <see cref="GridModel.HeaderRowCountChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before the header row count is changed."),
        Category("Grid")]
        public event GridCountChangingEventHandler HeaderRowCountChanging;

        /// <copyfrom cref="GridModel.FrozenRowCountChanged"/><summary>See <see cref="GridModel.FrozenRowCountChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after the frozen row count has been changed."),
        Category("Grid")]
        public event GridCountChangedEventHandler FrozenRowCountChanged;

        /// <copyfrom cref="GridModel.FrozenRowCountChanging"/><summary>See <see cref="GridModel.FrozenRowCountChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before the frozen row count is changed."),
        Category("Grid")]
        public event GridCountChangingEventHandler FrozenRowCountChanging;

        /// <copyfrom cref="GridModel.RowsMoved"/><summary>See <see cref="GridModel.RowsMoved"/> in the GridModel class for information.</summary>
        [Description("Occurs after a range of rows is moved."),
        Category("Grid")]
        public event GridRangeMovedEventHandler RowsMoved;

        /// <copyfrom cref="GridModel.RowsMoving"/><summary>See <see cref="GridModel.RowsMoving"/> in the GridModel class for information.</summary>
        [Description("Occurs before a range of rows is removed."),
        Category("Grid")]
        public event GridRangeMovingEventHandler RowsMoving;

        /// <copyfrom cref="GridModel.RowsRemoved"/><summary>See <see cref="GridModel.RowsRemoved"/> in the GridModel class for information.</summary>
        [Description("Occurs after a range of rows has been removed."),
        Category("Grid")]
        public event GridRangeRemovedEventHandler RowsRemoved;

        /// <copyfrom cref="GridModel.RowsRemoving"/><summary>See <see cref="GridModel.RowsRemoving"/> in the GridModel class for information.</summary>
        [Description("Occurs before a range of rows is removed."),
        Category("Grid")]
        public event GridRangeRemovingEventHandler RowsRemoving;

        /// <copyfrom cref="GridModel.RowsInserting"/><summary>See <see cref="GridModel.RowsInserting"/> in the GridModel class for information.</summary>
        [Description("Occurs before a range of rows is inserted."),
        Category("Grid")]
        public event GridRangeInsertingEventHandler RowsInserting;

        /// <copyfrom cref="GridModel.RowsInserted"/><summary>See <see cref="GridModel.RowsInserted"/> in the GridModel class for information.</summary>
        [Description("Occurs after a range of rows has been inserted."),
        Category("Grid")]
        public event GridRangeInsertedEventHandler RowsInserted;

        /// <copyfrom cref="GridModel.DefaultColWidthChanging"/><summary>See <see cref="GridModel.DefaultColWidthChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before the default column width is changed."),
        Category("Grid")]
        public event GridDefaultSizeChangingEventHandler DefaultColWidthChanging;

        /// <copyfrom cref="GridModel.DefaultColWidthChanged"/><summary>See <see cref="GridModel.DefaultColWidthChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after the default column width has been changed."),
        Category("Grid")]
        public event GridDefaultSizeChangedEventHandler DefaultColWidthChanged;

        /// <copyfrom cref="GridModel.HeaderColCountChanged"/><summary>See <see cref="GridModel.HeaderColCountChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after the header column count has been changed."),
        Category("Grid")]
        public event GridCountChangedEventHandler HeaderColCountChanged;

        /// <copyfrom cref="GridModel.HeaderColCountChanging"/><summary>See <see cref="GridModel.HeaderColCountChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before the header column count is changed."),
        Category("Grid")]
        public event GridCountChangingEventHandler HeaderColCountChanging;

        /// <copyfrom cref="GridModel.FrozenColCountChanged"/><summary>See <see cref="GridModel.FrozenColCountChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after the frozen column count has been changed."),
        Category("Grid")]
        public event GridCountChangedEventHandler FrozenColCountChanged;

        /// <copyfrom cref="GridModel.FrozenColCountChanging"/><summary>See <see cref="GridModel.FrozenColCountChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before the frozen column count is changed."),
        Category("Grid")]
        public event GridCountChangingEventHandler FrozenColCountChanging;

        /// <copyfrom cref="GridModel.ColsMoved"/><summary>See <see cref="GridModel.ColsMoved"/> in the GridModel class for information.</summary>
        [Description("Occurs after a range of columns is moved."),
        Category("Grid")]
        public event GridRangeMovedEventHandler ColsMoved;

        /// <copyfrom cref="GridModel.ColsMoving"/><summary>See <see cref="GridModel.ColsMoving"/> in the GridModel class for information.</summary>
        [Description("Occurs before a range of columns is removed."),
        Category("Grid")]
        public event GridRangeMovingEventHandler ColsMoving;

        /// <copyfrom cref="GridModel.ColsRemoved"/><summary>See <see cref="GridModel.ColsRemoved"/> in the GridModel class for information.</summary>
        [Description("Occurs after a range of columns has been inserted."),
        Category("Grid")]
        public event GridRangeRemovedEventHandler ColsRemoved;

        /// <copyfrom cref="GridModel.ColsRemoving"/><summary>See <see cref="GridModel.ColsRemoving"/> in the GridModel class for information.</summary>
        [Description("Occurs before a range of columns is removed."),
        Category("Grid")]
        public event GridRangeRemovingEventHandler ColsRemoving;

        /// <copyfrom cref="GridModel.ColsInserting"/><summary>See <see cref="GridModel.ColsInserting"/> in the GridModel class for information.</summary>
        [Description("Occurs before a range of columns is inserted."),
        Category("Grid")]
        public event GridRangeInsertingEventHandler ColsInserting;

        /// <copyfrom cref="GridModel.ColsInserted"/><summary>See <see cref="GridModel.ColsInserted"/> in the GridModel class for information.</summary>
        [Description("Occurs after a range of columns has been inserted."),
        Category("Grid")]
        public event GridRangeInsertedEventHandler ColsInserted;

        /// <copyfrom cref="GridModel.ReadOnlyChanged"/><summary>See <see cref="GridModel.ReadOnlyChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs when Read-only mode has changed."),
        Category("Grid")]
        public event EventHandler ReadOnlyChanged;

        /// <copyfrom cref="GridModel.SelectionChanging"/><summary>See <see cref="GridModel.SelectionChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before internal data structures are updated with new selection state from a SelectRange command."),
        Category("Grid")]
        public event GridSelectionChangingEventHandler SelectionChanging;

        /// <copyfrom cref="GridModel.SelectionChanged"/><summary>See <see cref="GridModel.SelectionChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after internal data structures were updated with new selection state from a SelectRange command."),
        Category("Grid")]
        public event GridSelectionChangedEventHandler SelectionChanged;

        /// <copyfrom cref="GridModel.PrepareClearSelection"/><summary>See <see cref="GridModel.PrepareClearSelection"/> in the GridModel class for information.</summary>
        [Description("Occurs before the grid will clear its list of selected ranges."),
        Category("Grid")]
        public event EventHandler PrepareClearSelection;

        /// <copyfrom cref="GridModel.PrepareChangeSelection"/><summary>See <see cref="GridModel.PrepareChangeSelection"/> in the GridModel class for information.</summary>
        [Description("Occurs before the grid will change the active selection range."),
        Category("Grid")]
        public event GridPrepareChangeSelectionEventHandler PrepareChangeSelection;

        /// <copyfrom cref="GridModel.CoveredRangesChanging"/><summary>See <see cref="GridModel.CoveredRangesChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before covering is applied or reset for a range of cells."),
        Category("Grid")]
        public event GridCoveredRangesChangingEventHandler CoveredRangesChanging;

        /// <copyfrom cref="GridModel.CoveredRangesChanged"/><summary>See <see cref="GridModel.CoveredRangesChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after covering was applied or reset for a range of cells."),
        Category("Grid")]
        public event GridCoveredRangesChangedEventHandler CoveredRangesChanged;

        /// <copyfrom cref="GridModel.BanneredRangesChanging"/><summary>See <see cref="GridModel.BanneredRangesChanging"/> in the GridModel class for information.</summary>
        [Description("Occurs before bannering is applied or reset for a range of cells."),
        Category("Grid")]
        public event GridBanneredRangesChangingEventHandler BanneredRangesChanging;

        /// <copyfrom cref="GridModel.BanneredRangesChanged"/><summary>See <see cref="GridModel.BanneredRangesChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after bannering was applied or reset for a range of cells."),
        Category("Grid")]
        public event GridBanneredRangesChangedEventHandler BanneredRangesChanged;

        /// <copyfrom cref="GridModel.FloatingCellsChanged"/><summary>See <see cref="GridModel.FloatingCellsChanged"/> in the GridModel class for information.</summary>
        [Description("Occurs after floating state was changed for a range of cells."),
        Category("Grid")]
        public event GridFloatingCellsChangedEventHandler FloatingCellsChanged;

        /// <copyfrom cref="GridModel.MergeCellsChanged"/>
        /// <summary>Occurs after floating state was changed for a range of cells.</summary>
        [Description("Occurs after floating state was changed for a range of cells."),
        Category("Grid")]
        public event GridMergeCellsChangedEventHandler MergeCellsChanged;

        /// <copyfrom cref="GridModel.QueryCanMergeCells"/>
        /// <summary>Occurs when grid compares the contents of two cells to determine if they should be merged.</summary>
        [Description("Occurs when grid compares the contents of two cells to determine if they should be merged."),
        Category("Grid")]
        public event GridQueryCanMergeCellsEventHandler QueryCanMergeCells;
        
        event GridChangeLayoutCellsEventHandler ChangingLayoutCells;
        event GridChangeLayoutCellsEventHandler ChangedLayoutCells;
        
        /// <copyfrom cref="GridModel.ClipboardCanCopy"/>
        /// <summary>Occurs when the grid's CanCopy method is called.</summary>
        [Description("Occurs when the grid's CanCopy method is called."),
        Category("Grid")]
        public event GridCutPasteEventHandler ClipboardCanCopy
        {
            add
            {
                Model.ClipboardCanCopy += value;
            }

            remove
            {
                Model.ClipboardCanCopy -= value;
            }
        }

        /// <copyfrom cref="GridModel.ClipboardCanPaste"/>
        /// <summary>Occurs when the grid's CanPaste method is called.</summary>
        [Description("Occurs when the grid's CanPaste method is called."),
        Category("Grid")]
        public event GridCutPasteEventHandler ClipboardCanPaste
        {
            add
            {
                Model.ClipboardCanPaste += value;
            }

            remove
            {
                Model.ClipboardCanPaste -= value;
            }
        }

        /// <copyfrom cref="GridModel.ClipboardCanCut"/>
        /// <summary>Occurs when the grid's CanCut method is called.</summary>
        [Description("Occurs when the grid's CanCut method is called."),
        Category("Grid")]
        public event GridCutPasteEventHandler ClipboardCanCut
        {
            add
            {
                Model.ClipboardCanCut += value;
            }

            remove
            {
                Model.ClipboardCanCut -= value;
            }
        }

        /// <copyfrom cref="GridModel.ClipboardCopy"/>
        /// <summary>Occurs when the grid's Copy method is called.</summary>
        [Description("Occurs when the grid's Copy method is called."),
        Category("Grid")]
        public event GridCutPasteEventHandler ClipboardCopy
        {
            add
            {
                Model.ClipboardCopy += value;
            }

            remove
            {
                Model.ClipboardCopy -= value;
            }
        }

        /// <copyfrom cref="GridModel.ClipboardCut"/>
        /// <summary>Occurs when the grid's Cut method is called.</summary>
        [Description("Occurs when the grid's Cut method is called."),
        Category("Grid")]
        public event GridCutPasteEventHandler ClipboardCut
        {
            add
            {
                Model.ClipboardCut += value;
            }

            remove
            {
                Model.ClipboardCut -= value;
            }
        }

        /// <copyfrom cref="GridModel.ClipboardPaste"/>
        /// <summary>Occurs when the grid's Paste method is called.</summary>
        [Description("Occurs when the grid's Paste method is called."),
        Category("Grid")]
        public event GridCutPasteEventHandler ClipboardPaste
        {
            add
            {
                Model.ClipboardPaste += value;
            }

            remove
            {
                Model.ClipboardPaste -= value;
            }
        }

        /// <copyfrom cref="GridModel.ClipboardCopyToBuffer"/>
        /// <summary>Occurs when the grid's CopyToBuffer method is called.</summary>
        [Description("Occurs when the grid's CopyToBuffer method is called."),
        Category("Grid")]
        public event ClipboardCopyToBufferEventHandler ClipboardCopyToBuffer
        {
            add
            {
                Model.ClipboardCopyToBuffer += value;
            }

            remove
            {
                Model.ClipboardCopyToBuffer -= value;
            }
        }

        /// <copyfrom cref="GridModel.ClearingCells"/>
        /// <summary>Occurs when the grid's ClearCells method is called.</summary>
        [Description("Occurs when the grid's ClearCells method is called."),
        Category("Grid")]
        public event GridClearingCellsEventHandler ClearingCells
        {
            add
            {
                Model.ClearingCells += value;
            }

            remove
            {
                Model.ClearingCells -= value;
            }
        }

        /// <copyfrom cref="GridModel.QueryCellText"/>
        /// <summary>Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value.</summary>
        [Description("Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler QueryCellText
        {
            add
            {
                Model.QueryCellText += value;
            }

            remove
            {
                Model.QueryCellText -= value;
            }
        }

        /// <copyfrom cref="GridModel.SaveCellText"/>
        /// <summary>Occurs each time the GridStyleInfo.Text is called to set the raw string that represents the underlying cell's value.</summary>
        [Description("Occurs each time the GridStyleInfo.Text is called to set the raw string that represents the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler SaveCellText
        {
            add
            {
                Model.SaveCellText += value;
            }

            remove
            {
                Model.SaveCellText -= value;
            }
        }

        /// <copyfrom cref="GridModel.QueryCellFormattedText"/>
        /// <summary>Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value.</summary>
        [Description("Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler QueryCellFormattedText
        {
            add
            {
                Model.QueryCellFormattedText += value;
            }

            remove
            {
                Model.QueryCellFormattedText -= value;
            }
        }

        /// <copyfrom cref="GridModel.SaveCellFormattedText"/>
        /// <summary>Occurs each time the GridStyleInfo.FormattedText is called to set the raw string that represents the underlying cell's value.</summary>
        [Description("Occurs each time the GridStyleInfo.FormattedText is called to set the raw string that represents the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler SaveCellFormattedText
        {
            add
            {
                Model.SaveCellFormattedText += value;
            }

            remove
            {
                Model.SaveCellFormattedText -= value;
            }
        }

        /// <copyfrom cref="GridModel.ParseCommonFormats"/>
        /// <summary>Handle this event to provide support for parsing the formatted string and convert it into the the underlying cell's value.</summary>
        [Description("Handle this event to provide support for parsing the formatted string and convert it into the the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler ParseCommonFormats
        {
            add
            {
                Model.ParseCommonFormats += value;
            }

            remove
            {
                Model.ParseCommonFormats -= value;
            }
        }

        /// <copyfrom cref="GridModel.ClipboardPaste"/>
        /// <summary>Occurs when a user starts dragging a range of selected cells using OLE drag-and-drop.</summary>
        [Description("Occurs when a user starts dragging a range of selected cells using OLE drag-and-drop."),
        Category("Grid")]
        public event GridQueryOleDataSourceDataEventHandler QueryOleDataSourceData
        {
            add
            {
                Model.QueryOleDataSourceData += value;
            }

            remove
            {
                Model.QueryOleDataSourceData -= value;
            }
        }
        
        /// <copyfrom cref="GridModel.GetSpannedRangeInfo"/><summary>See <see cref="GridModel.GetSpannedRangeInfo"/> in the GridModel class for information.</summary>
        public virtual bool GetSpannedRangeInfo(int rowIndex, int colIndex, out GridRangeInfo range)
        {
            bool result = Model.GetSpannedRangeInfo(rowIndex, colIndex, out range);
            return result;
        }

        /// <overload>
        /// Resumes the painting of the control suspended by calling the BeginUpdate method.
        /// </overload>
        /// <copyfrom cref="GridModel.EndUpdate()"/>
        /// <summary>See <see cref="GridModel.EndUpdate()"/> in the GridModel class for information.</summary>
        /// <param name="update">Specifies whether current pending paint operations should be discarded.</param>
        /// <param name="fromModel">Specified if this EndUpdate call was triggered by a call to the <see cref="GridModel.EndUpdate()"/>
        /// of the <see cref="GridModel"/></param>
        public override void EndUpdateModel(bool update, bool fromModel)
        {
            if (!fromModel)
            {
                Model.EndUpdate(update);
            }
            else
            {
                base.EndUpdateModel(update, fromModel);
            }
        }

        /// <copyfrom cref="GridModel.BeginUpdate()"/>
        /// <summary>
        /// Suspends the painting of associated grid controls until the EndUpdate method is called and records a command description
        /// why painting is suspended.
        /// </summary>
        /// <param name="options">Specifies the painting support during the BeginUpdate, EndUpdate batch.</param>
        /// <param name="commandDesc">A description of the command.</param>
        public void BeginUpdate(BeginUpdateOptions options, string commandDesc)
        {
            Model.BeginUpdate(options, commandDesc);
        }

        /// <copyfrom cref="GridModel.BeginUpdate()"/><summary>See <see cref="GridModel.BeginUpdate()"/> in the GridModel class for information.</summary>
        public override void BeginUpdateModel(BeginUpdateOptions options, bool fromModel)
        {
            if (!fromModel)
            {
                Model.BeginUpdate(options);
            }
            else
            {
                base.BeginUpdateModel(options, fromModel);
            }
        }

        /// <copyfrom cref="GridModel.SuspendChangeEvents"/><summary>See <see cref="GridModel.SuspendChangeEvents"/> in the GridModel class for information.</summary>
        public void SuspendChangeEvents()
        {
            Model.SuspendChangeEvents();
        }

        /// <copyfrom cref="GridModel.ResumeChangeEvents"/><summary>See <see cref="GridModel.ResumeChangeEvents"/> in the GridModel class for information.</summary>
        public void ResumeChangeEvents()
        {
            Model.ResumeChangeEvents();
        }

        /// <copyfrom cref="GridModel.SuspendRecordUndo"/><summary>See <see cref="GridModel.SuspendRecordUndo"/> in the GridModel class for information.</summary>
        public void SuspendRecordUndo()
        {
            Model.SuspendRecordUndo();
        }

        /// <copyfrom cref="GridModel.ResumeRecordUndo"/><summary>See <see cref="GridModel.ResumeRecordUndo"/> in the GridModel class for information.</summary>
        public void ResumeRecordUndo()
        {
            Model.ResumeRecordUndo();
        }

        /// <copyfrom cref="GridModel.BeginInit"/><summary>See <see cref="GridModel.BeginInit"/> in the GridModel class for information.</summary>
        public void BeginInit()
        {
            Model.BeginInit();
        }

        /// <copyfrom cref="GridModel.EndInit"/><summary>See <see cref="GridModel.EndInit"/> in the GridModel class for information.</summary>
        public void EndInit()
        {
            InitSplitterControl();
            Model.EndInit();
        }

        /// <copyfrom cref="GridModel.Initializing"/><summary>See <see cref="GridModel.Initializing"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Initializing
        {
            get
            {
                return Model.Initializing;
            }
        }

        /// <overload>See <see cref="GridModel.Refresh"/> in the GridModel class for information.</overload>
        /// <copyfrom cref="GridModel.Refresh"/><summary>See <see cref="GridModel.Refresh"/> in the GridModel class for information.</summary>
        protected override void Refresh(bool fromModel)
        {
            if (!fromModel)
            {
                Model.Refresh();
            }
            else
            {
                base.Refresh(fromModel);
            }
        }

        /// <copyfrom cref="GridModel.DoPrepareGraphics"/>
        /// <summary>
        /// Raise a <see cref="GridModel.PrepareGraphics"/> event.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        public void DoPrepareGraphics(Graphics g)
        {
            Model.DoPrepareGraphics(g);
        }

        ////        ///// <copyfrom cref="GridModel.CreateGraphics"/><summary>See <see cref="GridModel.CreateGraphics"/> in the GridModel class for information.</summary>
        ////        public Graphics CreateGraphics()
        ////        {
        ////            Graphics result = Model.CreateGraphics(this);
        ////            return result;
        ////        }
        ////        ///// <copyfrom cref="GridModel.CreateGraphics"/><summary>See <see cref="GridModel.CreateGraphics"/> in the GridModel class for information.</summary>
        ////        public Graphics CreateGraphics(Control c)
        ////        {
        ////            Graphics result = Model.CreateGraphics(c);
        ////            return result;
        ////        }

        /// <copyfrom cref="GridModel.LoadSoap(string)"/><summary>See <see cref="GridModel.LoadSoap(string)"/> in the GridModel class for information.</summary>
        public static GridModel LoadSoap(string fileName)
        {
            GridModel model = GridModel.LoadSoap(fileName);
            return model;
        }

        /// <copyfrom cref="GridModel.LoadSoap(string)"/><summary>See <see cref="GridModel.LoadSoap(string)"/> in the GridModel class for information.</summary>
        public static GridModel LoadSoap(Stream s)
        {
            GridModel model = GridModel.LoadSoap(s);
            return model;
        }

        ////        ///// <copyfrom cref="GridModel.LoadSoap"/><summary>See <see cref="GridModel.LoadSoap"/> in the GridModel class for information.</summary>
        ////        public void LoadSoap(string fileName)
        ////        {
        ////            Model = GridModel.LoadSoap(fileName);
        ////        }
        ////        ///// <copyfrom cref="GridModel.LoadSoap"/><summary>See <see cref="GridModel.LoadSoap"/> in the GridModel class for information.</summary>
        ////        public void LoadSoap(Stream s)
        ////        {
        ////            Model = GridModel.LoadSoap(s);
        ////        }

        /// <copyfrom cref="GridModel.SaveSoap()"/><summary>See <see cref="GridModel.SaveSoap()"/> in the GridModel class for information.</summary>
        public void SaveSoap()
        {
            Model.SaveSoap();
        }

        /// <copyfrom cref="GridModel.SaveSoap()"/><summary>See <see cref="GridModel.SaveSoap()"/> in the GridModel class for information.</summary>
        public void SaveSoap(string fileName)
        {
            Model.SaveSoap(fileName);
        }

        /// <copyfrom cref="GridModel.SaveSoap()"/><summary>See <see cref="GridModel.SaveSoap()"/> in the GridModel class for information.</summary>
        public void SaveSoap(Stream s)
        {
            Model.SaveSoap(s);
        }

        /// <copyfrom cref="GridModel.LoadBinary(string)"/><summary>See <see cref="GridModel.LoadBinary(string)"/> in the GridModel class for information.</summary>
        public static GridModel LoadBinary(string fileName)
        {
            GridModel model = GridModel.LoadBinary(fileName);
            return model;
        }

        /// <copyfrom cref="GridModel.LoadBinary(string)"/><summary>See <see cref="GridModel.LoadBinary(string)"/> in the GridModel class for information.</summary>
        public static GridModel LoadBinary(Stream s)
        {
            GridModel model = GridModel.LoadBinary(s);
            return model;
        }

        ////        ///// <copyfrom cref="GridModel.LoadBinary(string)"/><summary>See <see cref="GridModel.LoadBinary(string)"/> in the GridModel class for information.</summary>
        ////        public static void LoadBinary(string fileName)
        ////        {
        ////            Model = GridModel.LoadBinary(fileName);
        ////        }
        ////        ///// <copyfrom cref="GridModel.LoadBinary(string)"/><summary>See <see cref="GridModel.LoadBinary(string)"/> in the GridModel class for information.</summary>
        ////        public static void LoadBinary(Stream s)
        ////        {
        ////            Model = GridModel.LoadBinary(s);
        ////        }

        /// <copyfrom cref="GridModel.SaveBinary()"/><summary>See <see cref="GridModel.SaveBinary()"/> in the GridModel class for information.</summary>
        public void SaveBinary()
        {
            Model.SaveBinary();
        }

        /// <copyfrom cref="GridModel.SaveBinary()"/><summary>See <see cref="GridModel.SaveBinary()"/> in the GridModel class for information.</summary>
        public void SaveBinary(string fileName)
        {
            Model.SaveBinary(fileName);
        }

        /// <copyfrom cref="GridModel.SaveBinary()"/><summary>See <see cref="GridModel.SaveBinary()"/> in the GridModel class for information.</summary>
        public void SaveBinary(Stream s)
        {
            Model.SaveBinary(s);
        }

        /// <summary>
        ///     Recreates the <see cref="GridControl"/> with properties and data that was saved in xml format.
        /// </summary>
        public void InitializeFromXml(string fileName)
        {
            InitializeFromXml(fileName, true);
        }

        /// <summary>
        ///     Recreates the <see cref="GridControl"/> with properties and data that was saved in xml format.
        /// </summary>
        public void InitializeFromXml(string fileName, bool resolveBackgroundImages)
        {
            FileStream s = File.OpenRead(fileName);
            InitializeFromXml(s, resolveBackgroundImages);
            s.Close();
        }

        /// <summary>
        ///     Recreates the <see cref="GridControl"/> with properties and data that was saved in xml format.
        /// </summary>
        public void InitializeFromXml(Stream s)
        {
            InitializeFromXml(s, true);
        }

        /// <summary>
        ///     Recreates the <see cref="GridControl"/> with properties and data that was saved in xml format.
        /// </summary>
        public void InitializeFromXml(Stream s, bool resolveBackgroundImages)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Design.GridSyncProperties));
            object result = serializer.Deserialize(s);

            Design.GridSynchronizer gs = new Syncfusion.Windows.Forms.Grid.Design.GridSynchronizer();
            gs.InitializeGridSync(result as Design.GridSyncProperties, this);

            if (resolveBackgroundImages)
            {
                ////query the BackgroundImageID property and set the BackgroundImage property if necessary.
                ResolveBackgroundImages();
            }
        }

        /// <summary>
        /// Sets the <see cref="BackgroundImage"/> property if the <see cref="GridControlBase.BackgroundImageID()"/> property is set.
        /// </summary>
        private void ResolveBackgroundImages()
        {
            ////first the GridControls background
            if (this.BackgroundImageID != null && this.BackgroundImageID != string.Empty)
            {
                string idVal = this.BackgroundImageID;
                this.BackgroundImageID = string.Empty;

                idVal = Design.GridDesignerMain.syncProps.StoredImages.GetValueForName(idVal);
                Bitmap bmp = Design.GridSyncProperties.GetImageFromBytes(Convert.FromBase64String(idVal));
                this.BackgroundImage = bmp;
            }

            ////now the cells
            GridCellInfoCollection.GridCellInfoEnumerator cells = this.GridCells.GetEnumerator();
            while (cells.MoveNext())
            {
                if (cells.Current.StyleInfo != null)
                {
                    if (cells.Current.StyleInfo.BackgroundImageID != null && cells.Current.StyleInfo.BackgroundImageID != string.Empty)
                    {
                        string idVal = cells.Current.StyleInfo.BackgroundImageID;
                        cells.Current.StyleInfo.BackgroundImageID = string.Empty;

                        idVal = Design.GridDesignerMain.syncProps.StoredImages.GetValueForName(idVal);
                        Bitmap bmp = Design.GridSyncProperties.GetImageFromBytes(Convert.FromBase64String(idVal));
                        cells.Current.StyleInfo.BackgroundImage = bmp;
                    }
                }
            }
        }

        /// <summary>
        /// Saves the current <see cref="GridModel"/> object to a file in xml format. The filename can be specified with <see cref="GridModel.FileName"/>.
        /// </summary>
        public void SaveXml()
        {
            SaveXml(this.FileName);
        }

        /// <summary>
        /// Saves the current <see cref="GridModel"/> object in xml format to a file with the specified filename.
        /// </summary>
        public void SaveXml(string fileName)
        {
            this.FileName = fileName;
            FileStream s = File.Create(this.FileName);
            SaveXml(s);
            s.Close();
        }

        /// <summary>
        /// Saves the current <see cref="GridModel"/> object to a stream in xml format.
        /// </summary>
        public void SaveXml(Stream s)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Design.GridSyncProperties));
            Design.GridSynchronizer gs = new Syncfusion.Windows.Forms.Grid.Design.GridSynchronizer();
            gs.InitializeGridSyncProperties(this);
            Design.GridStylesParser.ParsePropertyStore(Design.GridDesignerMain.syncProps.Store);
            Design.GridDesignerMain.syncProps.Cells.InitializeFrom(this);
            serializer.Serialize(s, Design.GridDesignerMain.syncProps);
        }

        /// <copyfrom cref="GridModel.CalculatePreferredCellSize(System.Drawing.Graphics,int,int,Syncfusion.Windows.Forms.Grid.GridStyleInfo,Syncfusion.Windows.Forms.Grid.GridQueryBounds)"/><summary>See <see cref="GridModel.CalculatePreferredCellSize(System.Drawing.Graphics,int,int,Syncfusion.Windows.Forms.Grid.GridStyleInfo,Syncfusion.Windows.Forms.Grid.GridQueryBounds)"/> in the GridModel class for information.</summary>
        public Size CalculatePreferredCellSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size result = Model.CalculatePreferredCellSize(g, rowIndex, colIndex, style, queryBounds);
            return result;
        }

        /// <copyfrom cref="GridModel.CalculatePreferredCellSize(System.Drawing.Graphics,int,int,Syncfusion.Windows.Forms.Grid.GridStyleInfo,Syncfusion.Windows.Forms.Grid.GridQueryBounds)"/><summary>See <see cref="GridModel.CalculatePreferredCellSize(System.Drawing.Graphics,int,int,Syncfusion.Windows.Forms.Grid.GridStyleInfo,Syncfusion.Windows.Forms.Grid.GridQueryBounds)"/> in the GridModel class for information.</summary>
        public Size CalculatePreferredCellSize(Graphics g, int rowIndex, int colIndex, GridQueryBounds queryBounds)
        {
            Size result = Model.CalculatePreferredCellSize(g, rowIndex, colIndex, queryBounds);
            return result;
        }

        /// <copyfrom cref="GridModel.GetCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/><summary>See <see cref="GridModel.GetCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/> in the GridModel class for information.</summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <returns>A <see cref="GridStyleInfoStoreTable"/> object that holds contents for all the cells.</returns>
        public GridStyleInfoStoreTable GetCells(GridRangeInfo range)
        {
            GridStyleInfoStoreTable result = Model.GetCells(range);
            return result;
        }

        /// <copyfrom cref="GridModel.PopulateValues"/><summary>See <see cref="GridModel.PopulateValues"/> in the GridModel class for information.</summary>
        public void PopulateValues(GridRangeInfo range, object dataSource)
        {
            Model.PopulateValues(range, dataSource);
        }

        /// <copyfrom cref="GridModel.PopulateHeaders"/><summary>See <see cref="GridModel.PopulateHeaders"/> in the GridModel class for information.</summary>
        public void PopulateHeaders(GridRangeInfo range, object dataSource)
        {
            Model.PopulateHeaders(range, dataSource);
        }

        /// <copyfrom cref="GridModel.SetCellInfo(int,int,Syncfusion.Windows.Forms.Grid.GridStyleInfo,Syncfusion.Styles.StyleModifyType)"/><summary>See <see cref="GridModel.SetCellInfo(int,int,Syncfusion.Windows.Forms.Grid.GridStyleInfo,Syncfusion.Styles.StyleModifyType)"/> in the GridModel class for information.</summary>
        public bool SetCellInfo(int rowIndex, int colIndex, GridStyleInfo style, StyleModifyType modifyType)
        {
            return Model.SetCellInfo(rowIndex, colIndex, style, modifyType);
        }

        /// <copyfrom cref="GridModel.SetCellInfo(int,int,Syncfusion.Windows.Forms.Grid.GridStyleInfo,Syncfusion.Styles.StyleModifyType)"/><summary>See <see cref="GridModel.SetCellInfo(int,int,Syncfusion.Windows.Forms.Grid.GridStyleInfo,Syncfusion.Styles.StyleModifyType)"/> in the GridModel class for information.</summary>
        public bool SetCellInfo(int rowIndex, int colIndex, GridStyleInfo style, StyleModifyType modifyType, bool dontRaiseSaveCellInfoEvent, bool copyReferenceOnly)
        {
            return Model.SetCellInfo(rowIndex, colIndex, style, modifyType, dontRaiseSaveCellInfoEvent, copyReferenceOnly);
        }

        /// <copyfrom cref="GridModel.SetCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfoStoreTable)"/><summary>See <see cref="GridModel.SetCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfoStoreTable)"/> in the GridModel class for information.</summary>
        public void SetCells(GridRangeInfo range, GridStyleInfoStoreTable data)
        {
            Model.SetCells(range, data);
        }

        /// <copyfrom cref="GridModel.SetCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfoStoreTable)"/><summary>See <see cref="GridModel.SetCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfoStoreTable)"/> in the GridModel class for information.</summary>
        public void SetCells(GridRangeInfo range, GridStyleInfoStoreTable data, bool dontRaiseSaveCellInfoEvent, bool copyReferenceOnly)
        {
            Model.SetCells(range, data, dontRaiseSaveCellInfoEvent, copyReferenceOnly);
        }

        /// <copyfrom cref="GridModel.ResetVolatileData"/><summary>See <see cref="GridModel.ResetVolatileData"/> in the GridModel class for information.</summary>
        public void ResetVolatileData()
        {
            Model.ResetVolatileData();
        }

        /// <copyfrom cref="GridModel.GetCombinedStyle(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/><summary>See <see cref="GridModel.GetCombinedStyle(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/> in the GridModel class for information.</summary>
        public GridStyleInfo GetCombinedStyle(GridRangeInfo range)
        {
            GridStyleInfo result = Model.GetCombinedStyle(range);
            return result;
        }

        /// <copyfrom cref="GridModel.GetCombinedStyle(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/><summary>See <see cref="GridModel.GetCombinedStyle(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/> in the GridModel class for information.</summary>
        public GridStyleInfo GetCombinedStyle(GridRangeInfoList ranges)
        {
            GridStyleInfo result = Model.GetCombinedStyle(ranges);
            return result;
        }

        /// <copyfrom cref="GridModel.ChangeSelectionState"/>
        /// <summary>
        /// Records current selection state - current cell and selected ranges. Will be used for restoring selections when performing undo / redo,
        /// </summary>
        /// <param name="currentRow">The row index of current cell.</param>
        /// <param name="currentCol">The column index of current cell.</param>
        /// <param name="ranges">The current list of selected ranges.</param>
        public void ChangeSelectionState(int currentRow, int currentCol, GridRangeInfo[] ranges)
        {
            Model.ChangeSelectionState(currentRow, currentCol, ranges);
        }

        ////        ///// <copyfrom cref="GridModel.ScrollCellInView"/><summary>See <see cref="GridModel.ScrollCellInView"/> in the GridModel class for information.</summary>
        ////        public void ScrollCellInView(GridRangeInfo range)
        ////        {
        ////            Model.ScrollCellInView(range);
        ////        }

        /// <copyfrom cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/><summary>See <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/> in the GridModel class for information.</summary>
        public bool ChangeCells(GridRangeInfo range, string textValue)
        {
            bool result = Model.ChangeCells(range, textValue);
            return result;
        }

        /// <copyfrom cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/><summary>See <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/> in the GridModel class for information.</summary>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo cellInfo)
        {
            bool result = Model.ChangeCells(range, cellInfo);
            return result;
        }

        /// <copyfrom cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/><summary>See <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/> in the GridModel class for information.</summary>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo cellInfo, StyleModifyType modifyType)
        {
            bool result = Model.ChangeCells(range, cellInfo, modifyType);
            return result;
        }

        /// <copyfrom cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/><summary>See <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/> in the GridModel class for information.</summary>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo[] cellsInfo)
        {
            bool result = Model.ChangeCells(range, cellsInfo);
            return result;
        }

        /// <copyfrom cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/><summary>See <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/> in the GridModel class for information.</summary>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType modifyType)
        {
            bool result = Model.ChangeCells(range, cellsInfo, modifyType);
            return result;
        }

        /// <copyfrom cref="GridModel.GetCellsInfo(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/><summary>See <see cref="GridModel.GetCellsInfo(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/> in the GridModel class for information.</summary>
        public GridStyleInfo[] GetCellsInfo(GridRangeInfo range)
        {
            GridStyleInfo[] result = Model.GetCellsInfo(range);
            return result;
        }

        /// <copyfrom cref="GridModel.GetCellsInfo(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/><summary>See <see cref="GridModel.GetCellsInfo(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/> in the GridModel class for information.</summary>
        public GridStyleInfo[] GetCellsInfo(GridRangeInfo range, OperationFeedback op)
        {
            GridStyleInfo[] result = Model.GetCellsInfo(range, op);
            return result;
        }

        /// <copyfrom cref="GridModel.CanClearSelection"/>
        /// <summary>
        /// Checks if a range of cells is selected or if the grid has a current cell which contents can be cleared.
        /// </summary>
        /// <returns>True if clearing cells with <see cref="GridModel.Clear"/> is possible; False otherwise.</returns>
        /// <remarks>
        /// Use this to enable a "Clear" menu item or gray it out.
        /// </remarks>
        public bool CanClearSelection()
        {
            bool result = Model.CanClearSelection();
            return result;
        }

        /// <copyfrom cref="GridModel.Clear"/><summary>See <see cref="GridModel.Clear"/> in the GridModel class for information.</summary>
        public bool Clear(bool styleOrValue)
        {
            bool result = Model.Clear(styleOrValue);
            return result;
        }

        /// <copyfrom cref="GridModel.ClearCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,bool)"/><summary>See <see cref="GridModel.ClearCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,bool)"/> in the GridModel class for information.</summary>
        public bool ClearCells(GridRangeInfo range, bool bStyleOrValue)
        {
            bool result = Model.ClearCells(range, bStyleOrValue);
            return result;
        }

        /// <copyfrom cref="GridModel.ClearCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,bool)"/><summary>See <see cref="GridModel.ClearCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,bool)"/> in the GridModel class for information.</summary>
        public bool ClearCells(GridRangeInfoList selList, bool styleOrValue)
        {
            bool result = Model.ClearCells(selList, styleOrValue);
            return result;
        }

        /// <copyfrom cref="GridModel.ResetCurrentCellInfo"/><summary>See <see cref="GridModel.ResetCurrentCellInfo"/> in the GridModel class for information.</summary>
        public void ResetCurrentCellInfo()
        {
            Model.ResetCurrentCellInfo();
        }

        /// <copyfrom cref="GridModel.ConfirmChanges()"/><summary>See <see cref="GridModel.ConfirmChanges()"/> in the GridModel class for information.</summary>
        public void ConfirmChanges()
        {
            Model.ConfirmChanges();
        }

        /// <copyfrom cref="GridModel.EndEdit"/><summary>See <see cref="GridModel.EndEdit"/> in the GridModel class for information.</summary>
        public void EndEdit()
        {
            Model.EndEdit();
        }

        /// <copyfrom cref="GridModel.RaiseQueryMaximumRowCol"/><summary>See <see cref="GridModel.RaiseQueryMaximumRowCol"/> in the GridModel class for information.</summary>
        public void QueryMaximumRowCol(int nLastRow, int nLastCol)
        {
            Model.RaiseQueryMaximumRowCol(nLastRow, nLastCol);
        }

        /// <copyfrom cref="GridModel.ConfirmPendingChanges"/>
        /// <summary>
        /// Confirms any pending changes, raises the <see cref="GridModel.ConfirmingPendingChanges" /> event,
        /// and calls the current cells <see cref="GridCurrentCell.ConfirmChanges()"/> method.
        /// </summary>
        /// <returns>True if this action is successfully completed; False otherwise.</returns>
        public bool ConfirmPendingChanges()
        {
            bool result = Model.ConfirmPendingChanges();
            return result;
        }

        /// <copyfrom cref="GridModel.SubtractBorders(System.Drawing.Rectangle,Syncfusion.Windows.Forms.Grid.GridStyleInfo,bool)"/><summary>See <see cref="GridModel.SubtractBorders(System.Drawing.Rectangle,Syncfusion.Windows.Forms.Grid.GridStyleInfo,bool)"/> in the GridModel class for information.</summary>
        public Rectangle SubtractBorders(Rectangle cellBounds, GridStyleInfo style, bool isRightToLeft)
        {
            Rectangle result = Model.SubtractBorders(cellBounds, style, isRightToLeft);
            return result;
        }

        /// <copyfrom cref="GridModel.SubtractBorders(System.Drawing.Rectangle,Syncfusion.Windows.Forms.Grid.GridStyleInfo,bool)"/><summary>See <see cref="GridModel.SubtractBorders(System.Drawing.Rectangle,Syncfusion.Windows.Forms.Grid.GridStyleInfo,bool)"/> in the GridModel class for information.</summary>
        public Rectangle SubtractBorders(Rectangle cellBounds, GridStyleInfo style)
        {
            Rectangle result = Model.SubtractBorders(cellBounds, style, false);
            return result;
        }

        /// <summary>
        /// Adds border margins to given cell client area size. The borders are determined from a specified style with cell content information.
        /// </summary>
        /// <param name="size">The <see cref="System.Drawing.Size"/> with the cell size.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>The <see cref="System.Drawing.Size"/> with the cell size including its borders.</returns>
        public Size AddBorders(Size size, GridStyleInfo style)
        {
            Size result = Model.AddBorders(size, style);
            return result;
        }

        /// <copyfrom cref="GridModel.StyleInfoBordersToMargins"/><summary>See <see cref="GridModel.StyleInfoBordersToMargins"/> in the GridModel class for information.</summary>
        public GridMargins StyleInfoBordersToMargins(GridStyleInfo style)
        {
            GridMargins result = Model.StyleInfoBordersToMargins(style);
            return result;
        }

        /// <copyfrom cref="GridModel.SynchronizeCurrentCell"/><summary>See <see cref="GridModel.SynchronizeCurrentCell"/> in the GridModel class for information.</summary>
        public void SynchronizeCurrentCell(int rowIndex, int colIndex)
        {
            Model.SynchronizeCurrentCell(rowIndex, colIndex);
        }

        internal void FakeLButtonDown(Point pt, System.Windows.Forms.MouseButtons button)
        {
            this.OnMouseDown(new MouseEventArgs(button, 1, pt.X, pt.Y, 0));
        }

        internal void FakeLButtonUp(Point pt, System.Windows.Forms.MouseButtons button)
        {
            this.OnMouseUp(new MouseEventArgs(button, 1, pt.X, pt.Y, 0));
        }

        internal void FakeMouseMove(Point pt)
        {
            this.OnMouseMove(new MouseEventArgs(Control.MouseButtons, 0, pt.X, pt.Y, 0));
        }

        internal void FakeHScrollBarScroll(ScrollEventArgs args)
        {
            this.OnHScroll(this, args);
        }

        internal void FakeVScrollBarScroll(ScrollEventArgs args)
        {
            this.OnVScroll(this, args);
        }
#if SyncfusionFramework4_0
        /// <summary>
        /// Assigns the new UIAProvider for Accessibility.
        /// </summary>
        [Description("Assigns the new UIAProvider for Accessibility.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridUIAProvider Provider
        {
            get
            {
                return new GridUIAProvider(this);
            }
        }
#elif SyncfusionFramework3_5
        /// <summary>
        /// Assigns the new UIAProvider for Accessibility.
        /// </summary>
        [Description("Assigns the new UIAProvider for Accessibility.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridUIAProvider Provider
        {
            get
            {
                return new GridUIAProvider(this);
            }
        }
#endif
        /// <override/>
        protected override void WndProc(ref Message msg)
        {
#if SyncfusionFramework4_0
            if (msg.Msg == 0x003D /*Wmsg_GETOBJECT*/ && this.AccessibilityEnabled)
            {
                msg.Result = AutomationInteropProvider.ReturnRawElementProvider(Handle, msg.WParam, msg.LParam, this.Provider);
                return;
            }
#elif SyncfusionFramework3_5
            if (msg.Msg == 0x003D /*Wmsg_GETOBJECT*/ && this.AccessibilityEnabled)
            {
                msg.Result = AutomationInteropProvider.ReturnRawElementProvider(Handle, msg.WParam, msg.LParam, this.Provider);
                return;
            }
#endif
            base.WndProc(ref msg);
        }

        GridSerializeCellsBehavior serializeCellsBehavior = GridSerializeCellsBehavior.SerializeIntoCode;
        bool serializeCellsBehaviorModified = false;

        /// <summary>
        /// Gets or sets how to serialize grid data at design-time. You can choose to serialize
        /// cell contents as code into the Forms InitializeComponent method or you can choose
        /// to serialize cell contents into a ResX file. Default is GridSerializeCellsBehavior.SerializeIntoCode.
        /// </summary>
        /// <remarks>
        /// If you have a larger number of cells then you should switch to Resx serialization. For smaller
        /// number of cells SerializeIntoCode is easier to maintain since you can change cell contents
        /// directly within the Forms code.
        /// </remarks>
        [Category("Grid Contents")]
        [Description("Choose to serialize cell contents as code or into a ResX file.")]
        public GridSerializeCellsBehavior SerializeCellsBehavior
        {
            get
            {
                return this.serializeCellsBehavior;
            }

            set
            {
                this.serializeCellsBehavior = value;
                serializeCellsBehaviorModified = true;
            }
        }

        internal void ResetSerializeCellsBehavior()
        {
            this.serializeCellsBehavior = GridSerializeCellsBehavior.SerializeIntoCode;
            serializeCellsBehaviorModified = false;
        }

        internal bool ShouldSerializeSerializeCellsBehavior()
        {
            return this.serializeCellsBehaviorModified;
        }
    }

    /// <summary>
    /// Specifies how to serialize grid data at design-time. You can choose to serialize
    /// cell contents as code into the Forms InitializeComponent method or you can choose
    /// to serialize cell contents into a ResX file.
    /// </summary>
    public enum GridSerializeCellsBehavior
    {
        /// <summary>
        /// Serialize cell contents as code into the Forms InitializeComponent method 
        /// </summary>
        SerializeIntoCode,

        /// <summary>
        /// Serialize cell contents into a ResX file
        /// </summary>
        SerializeIntoResX,

        /// <summary>
        /// Serialize RangeStyles collection as code into the Forms InitializeComponent method 
        /// </summary>
        SerializeAsRangeStylesIntoCode,
    }
}
