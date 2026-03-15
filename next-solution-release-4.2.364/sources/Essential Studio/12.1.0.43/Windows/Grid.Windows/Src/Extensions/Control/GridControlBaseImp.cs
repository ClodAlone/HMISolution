//-------------------------------------------------------------------------------------------------
// <copyright file="GridControlBaseImp.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid.GridInternal;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements a grid control that displays a grid model.
    /// </summary>
    /// <remarks>
    /// <see cref="GridControlBase"/> implements a view on a <see cref="GridModel"/>. Several views can be opened for the same model. Changes
    /// in <see cref="GridModel"/> are reflected immediately among all views.<para/>
    /// The <see cref="GridModel"/> provides storage for all data and settings associated with the grid. The <see cref="GridControlBase"/>
    /// implements user interaction and display of the data.<para/>
    /// <see cref="GridControlBase"/> is a user control that is derived from <see cref="ScrollControl"/>. It lets the user scroll through grid data
    /// with mouse or keyboard. The grid displays a large number of cells where each cell can have its own unique formatting and cell type.<para/>
    /// <see cref="GridControlBase"/> also offers a wide range of events that let you customize the default behavior of the grid at run-time.
    /// <para/>
    /// <note type="note">In version 1.x, the GC class did derive directly from GridControlBase. Version 2.x derives GridControl from GridControlBaseImp. If you do have classes that derive directly from GridControlBase, your code will break. Please read the following explanation.
    /// <para/>
    /// If you derived directly from GridControlBase in your code, it is recommended that you change your code such that you derive from GridControlBaseImp instead.
    /// <para/>
    /// If you derived from the GridControl class, no action is required on your side.
    /// <para/>
    /// Here is why:
    /// <para/>
    /// In Version 1.x, the GridControlBase class did by default initialize all MouseControllers and CellTypes.
    /// <para/>
    /// We changed this now such that the GridControlBase class will only initialize the very basic cell types and mouse controllers. This step will help us later make the grid more modular and provide techniques to trim down the size of the grid assembly if you only need a subset of the features or also if we want to make parts of the grid work without interop code. The idea was to have a grid base class that only has minimal dependencies on other classes. If you have the grid source code and look at the grid\src tree, you will notice a "Base" folder and an "Extensions" folder. "Base" contains all essential files for a bare grid. "Extensions" contains all the files to make it a full-featured grid.
    /// <para/>
    /// In order to keep compatibility with existing code, the GridControlBaseImp class has been added. GridControlBaseImp is derived from GridControlBase and its sole purpose is to initialize all mouse controllers and cell types and establish other dependencies on grid features.
    /// <para/>
    /// If you derived directly from GridControlBase in your code base, it is recommended that you change your code such that you derive from GridControlBaseImp instead. Then your code should work without further change. Otherwise you need to manually initialize MouseControllers and cell types.
    /// <para/>
    /// The GridControl class itself has been changed such that it derives from GridControlBaseImp. Therefore if you were using the GridControl class no further change is necessary on your side.
    /// <para/>
    /// Here is a list of differences between GridControlBaseImp and GridControlBase:
    /// <list type="bullet">
    /// <item><term>
    /// GridControlBaseImp sets up the GridCellModelFactory as default cell type factory. GridControlBase on the other hand only sets up GridBaseCellModelFactory. GridCellModelFactory is able to instantiate the whole range of cell types that are part of Essential Grid. GridBaseCellModelFactory on the other hand only has dependencies on Header, Static, TextBox, ComboBox, and PushButton.
    /// <para/>
    /// So, if your code derives from GridControlBase directly and some celltypes do not appear to get instantiated, you should add cell types manually to the CellModels collection or derive from GridControlBaseImp.
    /// </term></item>
    /// <item><term>
    /// GridControlBaseImp overrides the InitializeMouseControllers method. In this override it checks Model.Options.ControllerOptions and instantiates all mouse controllers that have been specified in this flag. GridControlBase on the other hand does not perform any action in InitializeMouseControllers.
    /// </term></item>
    /// </list><para/>
    /// Other features that are implemented in GridControlBaseImp and not GridControlBase are:
    /// <list type="bullet">
    /// <item><term>
    /// The AccessibilityEnabled property. GridControlBase itself has no Accessibility support.
    /// </term></item>
    /// <item><term>
    /// Multiple cell selection.
    /// </term></item>
    /// <item><term>
    /// OLE drag-and-drop
    /// </term></item>
    /// <item><term>
    /// Cell tips
    /// </term></item>
    /// </list>
    /// <para/>
    /// Again, if you derived from GridControl, no action is required. If you derived from GridControlBase, please change your code to derive from GridControlBaseImp instead.
    /// <para/>
    /// <para/>
    /// The change is mainly a preparation to make it easier for us in future. We try hard not to break existing code. Providing a GridControlBaseImp class seemed like a good, easy solution and will help us in the future make the grid more modular.</note>
    /// <para/>
    /// <para/>
    /// </remarks>
    public class GridControlBaseImp : GridControlBase
    {
        #region Construct
        static GridControlBaseImp()
        {
            InitCellModelFactory();
        }

        /// <overload>
        /// Initializes a new <see cref="GridControlBaseImp"/>. 
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridControlBaseImp"/>. 
        /// </summary>
        public GridControlBaseImp()
            : this(null)
        {
            InitCellModelFactory();
        }

        /// <summary>
        /// Initializes a new <see cref="GridControlBaseImp"/> and attaches it to a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">The <see cref="GridModel"/> this control is associated with.</param>
        public GridControlBaseImp(GridModel model)
            : base(model)
        {
        }

        static void InitCellModelFactory()
        {
            if (GridFactoryProvider.CellModelFactory == null
                || GridFactoryProvider.CellModelFactory.IsDefault)
            {
                GridFactoryProvider.Init(new GridCellModelFactory());
            }
        }

        #endregion
        #region MouseControllers
        IMouseController resizeCellsController = null;
        IMouseController dragSelectRowOrColumnController = null;
        IMouseController selectCellsController = null;
        IMouseController dragColumnHeaderController;
        internal IMouseController clickCellsController = null;

        /// <summary>
        /// Initializes all mouse controllers for this grid. See <see cref="GridControllerOptions"/> for 
        /// default mouse controllers that you can enable and disable through the <see cref="GridModelOptions.ControllerOptions"/>
        /// of the <see cref="GridModel.Options"/> property.
        /// </summary>
        /// <remarks>
        /// Controllers will be added and removed from <see cref="ScrollControl.MouseControllerDispatcher"/>.
        /// </remarks>
        protected override void InitializeMouseControllers()
        {
            ////IMouseController controller;
            if ((Model.Options.ControllerOptions & GridControllerOptions.ResizeCells) != GridControllerOptions.None)
            {
                try
                {
                    if (resizeCellsController == null)
                    {
                        resizeCellsController = new GridResizeCellsMouseController(this);
                        MouseControllerDispatcher.Add(resizeCellsController);
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
            else if (resizeCellsController != null)
            {
                MouseControllerDispatcher.Remove(resizeCellsController);
                resizeCellsController = null;
            }

            if ((Model.Options.ControllerOptions & GridControllerOptions.DragSelectRowOrColumn) != GridControllerOptions.None)
            {
                try
                {
                    if (dragSelectRowOrColumnController == null)
                    {
                        dragSelectRowOrColumnController = new GridDragSelectMouseController(this);
                        MouseControllerDispatcher.Add(dragSelectRowOrColumnController);
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
            else if (dragSelectRowOrColumnController != null)
            {
                MouseControllerDispatcher.Remove(dragSelectRowOrColumnController);
                dragSelectRowOrColumnController = null;
            }

            if ((Model.Options.ControllerOptions & GridControllerOptions.SelectCells) != GridControllerOptions.None
                || (Model.Options.ControllerOptions & GridControllerOptions.ClickCells) != GridControllerOptions.None)
            {
                try
                {
                    if (selectCellsController == null)
                    {
                        selectCellsController = new GridSelectCellsMouseController(this);
                        MouseControllerDispatcher.Add(selectCellsController);
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
            else if (selectCellsController != null)
            {
                MouseControllerDispatcher.Remove(selectCellsController);
                selectCellsController = null;
            }

            if ((Model.Options.ControllerOptions & GridControllerOptions.ExcelLikeSelection) != GridControllerOptions.None)
            {
                try
                {
                    if (ExcelLikeFrameSelections == null)
                    {
                        ExcelLikeFrameSelections = new GridPaintExcelLikeSelection(this);
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
            else if (ExcelLikeFrameSelections != null)
            {
                ExcelLikeFrameSelections = null;
            }

            ////oleDataSource
            if ((Model.Options.ControllerOptions & GridControllerOptions.OleDataSource) != GridControllerOptions.None)
            {
                try
                {
                    EnableOleDataSource();
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
            else if (oleDataSource != null)
            {
                MouseControllerDispatcher.Remove(oleDataSource);
                oleDataSource = null;
            }

            if ((Model.Options.ControllerOptions & GridControllerOptions.OleDropTarget) != GridControllerOptions.None)
            {
                try
                {
                    EnableOleDropTarget();
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
            else if (oleDropTarget != null)
            {
                oleDropTarget.Dispose();
                oleDropTarget = null;
            }

            this.InitializeDataObjectConsumerOptions();

            if ((Model.Options.ControllerOptions & GridControllerOptions.DragColumnHeader) != GridControllerOptions.None)
            {
                try
                {
                    if (dragColumnHeaderController == null)
                    {
                        dragColumnHeaderController = new GridDragColumnHeaderMouseController(this);
                        MouseControllerDispatcher.Add(dragColumnHeaderController);
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
            else if (dragColumnHeaderController != null)
            {
                MouseControllerDispatcher.Remove(dragColumnHeaderController);
                dragColumnHeaderController = null;
            }
        }

        /// <summary>
        /// Resets all mouse controllers and removes them from <see cref="ScrollControl.MouseControllerDispatcher"/>.
        /// </summary>
        protected override void ResetMouseControllers()
        {
            MouseControllerDispatcher.Remove(resizeCellsController);
            MouseControllerDispatcher.Remove(dragSelectRowOrColumnController);
            MouseControllerDispatcher.Remove(selectCellsController);
            MouseControllerDispatcher.Remove(clickCellsController);
            MouseControllerDispatcher.Remove(oleDataSource);
            resizeCellsController = null;
            dragSelectRowOrColumnController = null;
            selectCellsController = null;
            clickCellsController = null;
            oleDataSource = null;
        }

        IGridDataObjectConsumer dataDataObjectConsumer = null;
        IGridDataObjectConsumer textDataObjectConsumer = null;

        /// <summary>
        /// Initializes all data object consumers for this grid. See <see cref="GridDataObjectConsumerOptions"/> for 
        /// default consumers that you can enable and disable through the <see cref="GridModelOptions.DataObjectConsumerOptions"/> property
        /// of the <see cref="GridModel.Options"/> property.
        /// </summary>
        /// <remarks>
        /// Controllers will be registered by the <see cref="RegisterDataObjectConsumer"/>.
        /// </remarks>
        protected override void InitializeDataObjectConsumerOptions()
        {
            if ((Model.Options.ControllerOptions & GridControllerOptions.OleDropTarget) != GridControllerOptions.None)
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
        #endregion
        #region WireModel
        /// <summary>
        /// Sets up listeners for the <see cref="GridModel"/> and initializes mouse controllers and data object consumers.
        /// </summary>
        protected override void WireModel()
        {
            base.WireModel();

            Model.SelectionChanged += new GridSelectionChangedEventHandler(ModelSelectionChanged);
            Model.PrepareClearSelection += new EventHandler(ModelPrepareClearSelection);
            Model.PrepareChangeSelection += new GridPrepareChangeSelectionEventHandler(ModelPrepareChangeSelection);
        }

        /// <summary>
        /// Releases listeners for the <see cref="GridModel"/>.
        /// </summary>
        protected override void UnwireModel()
        {
            base.UnwireModel();

            if (Model != null)
            {
                Model.SelectionChanged -= new GridSelectionChangedEventHandler(ModelSelectionChanged);
                Model.PrepareClearSelection -= new EventHandler(ModelPrepareClearSelection);
                Model.PrepareChangeSelection -= new GridPrepareChangeSelectionEventHandler(ModelPrepareChangeSelection);
            }
        }

        #endregion
        #region PaintSelectCells
        private GridPaintSelectCells paintSelectCells = null;

        /// <override/>
        protected override void IntUpdateSelectRange()
        {
            if (UpdateSelectRange_Range != null && !UpdateSelectRange_Range.IsEmpty)
            {
                PaintSelectCells.UpdateSelectRange(UpdateSelectRange_Range, UpdateSelectRange_OldRange);
                UpdateSelectRange_Range = null;
                UpdateSelectRange_OldRange = null;
            }
        }

        /// <override/>
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IGridPaintSelectCells PaintSelectCells
        {
            get
            {
                if (paintSelectCells == null)
                {
                    paintSelectCells = new GridPaintSelectCells(this);
                }

                return paintSelectCells;
            }
        }

        void ModelSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (e.OldRanges != null)
            {
                if (PaintSelectCells != null)
                {
                    PaintSelectCells.UpdateSelectRange(e.Range, e.OldRanges);
                }
            }
        }

        void ModelPrepareClearSelection(object sender, EventArgs e)
        {
            if (PaintSelectCells != null)
            {
                PaintSelectCells.PrepareClearSelection();
            }
        }

        void ModelPrepareChangeSelection(object sender, GridPrepareChangeSelectionEventArgs e)
        {
            if (PaintSelectCells != null)
            {
                PaintSelectCells.PrepareChangeSelection(e.OldRange, e.NewRange);
            }
        }

        #endregion
        #region OleDragDrop
        private GridOleDataSourceMouseController oleDataSource = null;

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
        public bool EnableOleDataSource(int flags)
        {
            if (oleDataSource != null)
            {
                this.MouseControllerDispatcher.Remove(oleDataSource);
            }

            oleDataSource = new GridOleDataSourceMouseController(this);
            if (oleDataSource != null)
            {
                this.MouseControllerDispatcher.Add(oleDataSource);
                return oleDataSource.EnableOleDataSource(flags);
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
            int flags = this.Model.Options.DragDropDropTargetFlags;
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
        public void EnableOleDropTarget(int flags)
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
        protected virtual GridOleDropTarget CreateOleDropTarget(GridControlBase grid, int flags)
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
        #region Accesibility

        internal GridControlRowAccessibleObject CreateRowAccessibilityInstance(int index)
        {
            ////TraceUtil.TraceCurrentMethodInfo(index);
            return new GridControlRowAccessibleObject(this, index + 1);
        }

        GridControlRowAccessibleObjectsIndexer recordAccessibleObjects = null;

        internal GridControlRowAccessibleObjectsIndexer RowAccessibleObjects
        {
            get
            {
                if (recordAccessibleObjects == null)
                {
                    recordAccessibleObjects = new GridControlRowAccessibleObjectsIndexer(this);
                }

                return recordAccessibleObjects;
            }
        }

        internal GridControlColHeaderAccessibleObject CreateColHeaderAccessibilityInstance(int index)
        {
            ////TraceUtil.TraceCurrentMethodInfo(index);
            return new GridControlColHeaderAccessibleObject(this, index + 1);
        }

        GridControlColHeaderAccessibleObjectsIndexer colHeaderAccessibleObjects = null;

        internal GridControlColHeaderAccessibleObjectsIndexer ColHeaderAccessibleObjects
        {
            get
            {
                if (colHeaderAccessibleObjects == null)
                {
                    colHeaderAccessibleObjects = new GridControlColHeaderAccessibleObjectsIndexer(this);
                }

                return colHeaderAccessibleObjects;
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
        /// Creates a new accessibility object for the control.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Windows.Forms.AccessibleObject"/> for the control.
        /// </returns>
        /// <override/>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            if (accessibilityEnabled)
            {
                // Overridden to return the custom AccessibleObject 
                // for the entire grid.
                return new GridControlBaseAccessibleObject(this);
            }

            return base.CreateAccessibilityInstance();
        }
        #endregion
        #region Cell Tips

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (this.cellToolTip1 != null)
                {
                    this.cellToolTip1.RemoveAll();
                    this.cellToolTip1.Dispose();
                    this.cellToolTip1 = null;
                }

                if (this.excelLikeFrameSelections is IDisposable)
                {
                    ((IDisposable)this.excelLikeFrameSelections).Dispose();
                }

                if (this.dataDataObjectConsumer is IDisposable)
                {
                    ((IDisposable)this.dataDataObjectConsumer).Dispose();
                }

                if (this.oleDataSource != null)
                {
                    this.oleDataSource.Dispose();
                }

                if (this.oleDropTarget != null)
                {
                    this.oleDropTarget.Dispose();
                }

                if (this.paintSelectCells != null)
                {
                    this.paintSelectCells.Dispose();
                }

                if (this.textDataObjectConsumer is IDisposable)
                {
                    ((IDisposable)this.textDataObjectConsumer).Dispose();
                }
            }
        }

        private ToolTip cellToolTip1 = null;
        private int tipRow = -1;
        private int tipCol = -1;

        /// <summary>
        /// Gets or sets a reference to the ToolTip object used for displaying
        /// <see cref="GridStyleInfo.CellTipText"/> as ToolTips for cells.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolTip CellToolTip
        {
            get
            {
                if (cellToolTip1 == null)
                {
                    cellToolTip1 = CreateCellToolTip();
                    cellToolTip1.Disposed += new EventHandler(cellToolTip1_Disposed);
                    ////                    Form parentForm = FindFormHelper.FindForm(this);
                    ////                    if (parentForm != null)
                    ////                        parentForm.Closed += new EventHandler(parentForm_Closed);
                }

                return cellToolTip1;
            }

            set
            {
                if (cellToolTip1 != value)
                {
                    if (cellToolTip1 != null)
                    {
                        cellToolTip1.RemoveAll();
                        cellToolTip1.Dispose();
                    }

                    cellToolTip1 = value;
                    cellToolTip1.Disposed += new EventHandler(cellToolTip1_Disposed);
                }
            }
        }

        /// <summary>
        /// Creates and initializes a ToolTip object. InitialDelay will be 500 and
        /// ReshowDelay will be set to 0 by default.
        /// </summary>
        /// <returns>The initialized ToolTip object for this grid.</returns>
        protected virtual ToolTip CreateCellToolTip()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.InitialDelay = 500; ////half a second delay 
            toolTip.ReshowDelay = 0;
            return toolTip;
        }
        /// <overide/>
        protected override void Refresh(bool fromModel)
        {
            tipCol = -1;
            tipRow = -1;
            base.Refresh(fromModel);
        }

        internal void OnCellTipsMouseMove(MouseEventArgs e)
        {
            int row, col;
            Point pt = new Point(e.X, e.Y);
            if (PointToRowCol(new Point(e.X, e.Y), out row, out col))
            {
                CurrentCell.AdjustRowColIfCoveredCell(ref row, ref col);
            }

            if (IntelliMouseDragScroll.ActiveIntelliMouseDragScroll != null)
            {
                row = -1;
            }

            if (col != tipCol || row != tipRow)
            {
                ToolTip toolTip = ((GridControlBaseImp)this.GetGridWindow()).CellToolTip;
                tipCol = col;
                tipRow = row;
                if (toolTip != null && toolTip.Active)
                {
                    toolTip.Active = false; ////turn it off 
                }

                if (row == -1)
                {
                    return;
                }

                GridStyleInfo style = GetViewStyleInfo(row, col);
                GridActivateToolTipEventArgs ae = new GridActivateToolTipEventArgs(row, col, style);
                OnActivateToolTip(ae);
                if (!ae.Cancel && !GridUtil.IsEmpty(style.CellTipText))
                {
                    toolTip.RemoveAll();
                    toolTip.Show(style.CellTipText, GetGridWindow());
                    toolTip.Active = true; ////make it active so it can show 
                }
                style.Dispose();
            }
        }

        /// <summary>
        /// Occurs when mouse has moved to a new cell and a ToolTip is initialized for that cell.
        /// </summary>
        /// <remarks>
        /// You have two options: <para/>
        /// 1) Set e.Style.CellTipText<para/>
        /// - Or - <para/>
        /// 2) Initalize the ToolTip directly (see <see cref="CellToolTip"/> property) and then
        /// set e.Cancel = true;<para/>
        /// </remarks>
        [Category("Grid")]
        [Description("Occurs when mouse pointer is moved to a new cell and a ToolTip is initialized for that cell.")]
        public event GridActivateToolTipEventHandler ActivateToolTip;

        /// <summary>
        /// Raises the  <see cref="ActivateToolTip"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridActivateToolTipEventArgs" /> that contains the event data.</param>
        protected virtual void OnActivateToolTip(GridActivateToolTipEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, Name, e);
            if (ActivateToolTip != null)
            {
                ActivateToolTip(this, e);
            }
        }

        /// <override/>
        protected override/*Control*/ void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            OnCellTipsMouseMove(e);
        }

        private void cellToolTip1_Disposed(object sender, EventArgs e)
        {
            cellToolTip1.Disposed -= new EventHandler(cellToolTip1_Disposed);
            this.cellToolTip1 = null;
        }

        ////        private void parentForm_Closed(object sender, EventArgs e)
        ////        {
        ////            if (this.cellToolTip1 != null)
        ////                this.cellToolTip1.Dispose();
        ////            this.cellToolTip1 = null;
        ////            Form parentForm = sender as Form;
        ////            if (parentForm != null)
        ////                parentForm.Closed -= new EventHandler(parentForm_Closed);
        ////        }

        #endregion
        #region QueryAllowDragColumnHeader
        // event GridQueryDragColumnHeaderEventHandler QueryAllowDragColumnHeader;

        /// <summary>
        /// Occurs when the user hovers the mouse over a column header or clicks on it.
        /// In your event handler, you can determine if the selected column can be dragged.
        /// </summary>
        /// <remarks>
        /// You can disallow dragging the column when
        /// you assign False to <see cref="GridQueryDragColumnHeaderEventArgs.AllowDrag"/>.
        /// </remarks>
        /// <seealso cref="GridQueryDragColumnHeaderEventArgs"/>
        [Category("Drag Drop")]
        [Description("Occurs when the user hovers the mouse over a column header or clicks on it.")]
        public event GridQueryDragColumnHeaderEventHandler QueryAllowDragColumnHeader;

        /// <summary>
        /// Raises the <see cref="OnQueryAllowDragColumnHeader"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryDragColumnHeaderEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryAllowDragColumnHeader(GridQueryDragColumnHeaderEventArgs e)
        {
            if (QueryAllowDragColumnHeader != null)
            {
                QueryAllowDragColumnHeader(this, e);
            }
        }

        internal void RaiseQueryAllowDragColumn(GridQueryDragColumnHeaderEventArgs e)
        {
            OnQueryAllowDragColumnHeader(e);
        }
        #endregion
    }
}
