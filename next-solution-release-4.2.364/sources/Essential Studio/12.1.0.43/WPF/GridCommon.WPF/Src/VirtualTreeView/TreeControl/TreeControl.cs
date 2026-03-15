#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;

#if ENABLE_PARTIAL_TRUST
using System.Security;
#endif

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// The VirtualTreeView displays a hierarchical collection of expandable <see cref="TreeNode"/>
    /// items maintained by a <see cref="TreeModel"/>. A flattened representations of the hierarchy 
    /// of nodes can be accessed through
    /// the <see cref="TreeModel.VisibleNodes"/> property of the <see cref="Model"/> where each visible node is mapped to a row index
    /// and vice versa. <see cref="TreeNodesFlattenedList"/> also implements <see cref="Syncfusion.Windows.Controls.Scroll.ILineSizeHost"/>
    /// and the <see cref="VirtualTreeView"/> assigns it to <see cref="Syncfusion.Windows.Controls.Scroll.ScrollAxisControl.RowHeightsProvider"/>
    /// in order to be able to pixel scroll through visible and expanded nodes. 
    /// <para/>
    /// VirtualTreeView returns cell contents with the <see cref="GetRenderStyleInfo(RowColumnIndex)"/>
    /// method which returns a <see cref="TreeRenderStyleInfo"/> object. Each cell is associated
    /// with a <see cref="TreeCellRenderer"/> object which provides methods for measuring,
    /// arranging and drawing contents of a cell. The VirtualTreeView implements
    /// the <see cref="ScrollControl.OnArrangeContent"/> and <see cref="UIElement.OnRender"/> methods. Within implementation
    /// of the OnArrangeContent method each cells UIElement children are placed on the controls area.
    /// A cell renderer can also be without any UIElement children and instead draw all its contents
    /// directly to the DrawingContext of the VirtualizingCellsControl when the <see cref="UIElement.OnRender"/>
    /// method is executed. For a cell renderer it is also possible to do both: Arrange UIElements
    /// on the controls area and draw additional contents in its render area. There are also
    /// various optimization techniques that can be implemented with the renderer and are discussed
    /// in the <see cref="ICellRenderer"/> overview.
    /// <para/>
    /// The virtualization of UIElement children of cell renderers is implemented in 
    /// the arrange cells code. At the time a cell is placed the cell renderer is 
    /// called to create and intialize the UIElement children. When a cell is scrolled
    /// out of view the cell renderer is called to unload the UIElement children. A cell
    /// renderer can decide whether to unload a UIElement, keep it alive or move
    /// it to a recycle bin and reuse it later. The main logic of this code is implemented
    /// by the <see cref="TreeVirtualizingCellRendererBase{T}"/> class which contains more detailed
    /// discussion about this feature.    
    /// </summary>          
    
#if ENABLE_PARTIAL_TRUST
    [SecuritySafeCritical]
#endif
    public class VirtualTreeView : VirtualizingCellsControl
    {
        #region Fields
        TreeModel model = null;
        TreeCellStaticTextRenderer staticTextRenderer = new TreeCellStaticTextRenderer();
        TreeCellRenderer treeNodeRenderer = new TreeCellNodeRenderer(); // (uses checkbox to collapse/expand ...)
        TreeCellRenderer treeColumnHeaderRenderer = new TreeColumnHeaderRenderer(); // (uses checkbox to collapse/expand ...)

        // Virtualized TreeStyleInfo
        TreeControlRenderStyles renderStyles;
        #endregion

        #region Dependency Properties

        #region RenderCellInfo
        /// <summary>
        /// Gets the <see cref="VirtualizingCellsControl.RenderCellInfoProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static TreeRenderStyleInfo GetRenderStyleInfo(DependencyObject dpo)
        {
            return GetRenderCellInfo(dpo) as TreeRenderStyleInfo;
        }

        /// <summary>
        /// Sets the <see cref="VirtualizingCellsControl.RenderCellInfoProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetRenderStyleInfo(DependencyObject dpo, TreeRenderStyleInfo value)
        {
            SetRenderCellInfo(dpo, value);
        }
        #endregion

        #region CellRendererProperty

        /// <summary>
        /// Gets the <see cref="VirtualizingCellsControl.CellRendererProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static new TreeCellRenderer GetCellRenderer(DependencyObject dpo)
        {
            return GetCellRenderer(dpo) as TreeCellRenderer;
        }

        /// <summary>
        /// Sets the <see cref="VirtualizingCellsControl.CellRendererProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetCellRenderer(DependencyObject dpo, TreeCellRenderer value)
        {
            SetCellRenderer(dpo, value);
        }
        #endregion

        #region NodeTemplateProperty
        /// <summary>
        /// Defines the DataTemplate to use with the expandable node renderer.
        /// </summary>
        public static readonly DependencyProperty NodeTemplateProperty = DependencyProperty.Register("NodeTemplate", typeof(DataTemplate), typeof(VirtualTreeView));

        /// <summary>
        /// Gets or sets the DataTemplate to use with the expandable node renderer.
        /// </summary>
        /// <value>The node template.</value>
        public DataTemplate NodeTemplate
        {
            get { return (DataTemplate)GetValue(NodeTemplateProperty); }
            set { SetValue(NodeTemplateProperty, value); }
        }
        #endregion

        #endregion

        #region Ctor
        static VirtualTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualTreeView), new FrameworkPropertyMetadata(typeof(VirtualTreeView)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualTreeView"/> class.
        /// </summary>
        public VirtualTreeView()
        {
            renderStyles = new TreeControlRenderStyles(this);
            Focusable = true;

            MouseControllerDispatcher.Add(new TreeResizeColumnsMouseController(this));
            MouseControllerDispatcher.Add(new TreeResizeRowsMouseController(this));
        }

        #endregion

        #region Model
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public TreeModel Model
        {
            get
            {
                if (model == null)
                    Model = new TreeModel();
                return model;
            }
            set
            {
                if (model != value)
                {
                    if (model != null)
                    {
                        model.NodeCollapsed -= new TreeNodeEventHandler(model_NodeCollapsed);
                        model.NodeExpanded -= new TreeNodeEventHandler(model_NodeExpanded);
                        model.LineCountChanged -= new System.EventHandler(model_LineCountChanged);
                    }
                    model = value;
                    ColumnWidthsProvider = Model.Columns;
                    RowHeightsProvider = Model.VisibleNodes;
                    model.NodeCollapsed += new TreeNodeEventHandler(model_NodeCollapsed);
                    model.NodeExpanded += new TreeNodeEventHandler(model_NodeExpanded);
                    model.LineCountChanged += new System.EventHandler(model_LineCountChanged);
                }
            }
        }

        void model_LineCountChanged(object sender, System.EventArgs e)
        {
            RefreshDisplay();
        }

        void model_NodeExpanded(object sender, TreeNodeEventArgs e)
        {
            RaiseNodeExpanded(e);
        }

        void model_NodeCollapsed(object sender, TreeNodeEventArgs e)
        {
            RaiseNodeCollapsed(e);
        }

        #endregion

        #region TreeNodesScrollAxis
        /// <summary>
        /// Creates the row or column scroll axis. The default implementation of this method creates either a
        /// <see cref="PixelScrollAxis"/> or <see cref="LineScrollAxis"/> object. You can override this method
        /// if you want to add support for another custom tailored scroll axis object.
        /// </summary>
        /// <param name="orientation">The orientation (Vertical for row scrolling, Horizontal for column scrolling)</param>
        /// <param name="pixelScroll">if set to <c>true</c> pixel scroll; otherwise line scrolling.</param>
        /// <param name="scrollBar">The state of the scroll bar.</param>
        /// <param name="lineSizes">An object that provides row or column sizes.</param>
        /// <returns>The scroll axis object.</returns>
        protected override ScrollAxisBase CreateScrollAxis(Orientation orientation, bool pixelScroll, IScrollBar scrollBar, ILineSizeHost lineSizes)
        {
            if (orientation == Orientation.Vertical)
                return new TreeNodesScrollAxis(scrollBar, Model);

            return base.CreateScrollAxis(orientation, pixelScroll, scrollBar, lineSizes);
        }

        #endregion

        #region TreeNode events
        /// <summary>
        /// Occurs when node was expanded.
        /// </summary>
        public event TreeNodeEventHandler NodeExpanded;

        /// <summary>
        /// Occurs when a node was collapsed.
        /// </summary>
        public event TreeNodeEventHandler NodeCollapsed;


        /// <summary>
        /// Raises the <see cref="NodeExpanded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="TreeNodeEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeExpanded(TreeNodeEventArgs e)
        {
            if (NodeExpanded != null)
                NodeExpanded(this, e);
        }

        /// <summary>
        /// Raises the <see cref="NodeCollapsed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="TreeNodeEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeCollapsed(TreeNodeEventArgs e)
        {
            if (NodeCollapsed != null)
                NodeCollapsed(this, e);
        }

        #endregion

        #region Virtualized Cell Styles

        /// <summary>
        /// Returns a container object which manages cell styles (<see cref="TreeRenderStyleInfo"/>)
        /// of rendered cells that have been scrolled into view.<para/>
        /// </summary>
        public TreeControlRenderStyles RenderStyles
        {
            get { return renderStyles; }
        }
        
        /// <summary>
        /// Arranges the cells row by row. For each cell
        /// the virtual <see cref="VirtualizingCellsControl.OnArrangeCell"/> method is called. OnArrangeCell gets
        /// the <see cref="ICellRenderer"/> for a cell and calls its <see cref="ICellRenderer.Arrange"/>
        /// method.<para/>
        /// The method also implements the virtualization of UIElement children of cell renderers.
        /// It create new UIElement objects for cells scrolled into view or unload UIElements for
        /// cells scrolled out of view. If a UIElement has focus whenscrolled out of view it will
        /// be kept alive and not unloaded.
        /// </summary>
        /// <param name="arrangeSize"></param>
        protected override void ArrangeCellUIElements(Size arrangeSize)
        {
            // Create new TreeStyleInfo objects for cells scrolled into view or unload TreeStyleInfo objects for cells scrolled out of view.
            RenderStyles.PrepareArrange();

            base.ArrangeCellUIElements(arrangeSize);

            RenderStyles.ConcludeArrange();
        }

        /// <summary>
        /// Gets the render cell style for a cell. VirtualizingCellsControl solely relies
        /// on the <see cref="IRenderCellInfo"/> for drawing and renderer information
        /// of a cell. Concrete implementations of this interface such as GridRenderStyleInfo
        /// or TreeRenderStyleInfo can add support for additional domain specific
        /// properties.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        protected internal override IRenderCellInfo GetRenderCellInfo(int rowIndex, int columnIndex)
        {
            return RenderStyles.GetRenderStyleInfo(rowIndex, columnIndex);
        }

        

        /// <summary>
        /// Gets the cell renderer from a render cell style.
        /// </summary>
        /// <param name="cellInfo">The render cell style.</param>
        /// <returns></returns>
        protected override internal ICellRenderer GetCellRenderer(IRenderCellInfo cellInfo)
        {
            ICellRenderer renderer = ((TreeStyleInfo)cellInfo).CellRenderer;
            if (renderer == null)
                renderer = staticTextRenderer;
            return renderer;
        }

        /// <summary>
        /// Gets the render cell style for a cell. VirtualizingCellsControl solely relies
        /// on the <see cref="IRenderCellInfo"/> for drawing and renderer information
        /// of a cell. Concrete implementations of this interface such as GridRenderStyleInfo
        /// or TreeRenderStyleInfo can add support for additional domain specific
        /// properties.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        public TreeRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex)
        {
            return RenderStyles.GetRenderStyleInfo(rowIndex, columnIndex);
        }

        /// <summary>
        /// Gets the render cell style for a cell. VirtualizingCellsControl solely relies
        /// on the <see cref="IRenderCellInfo"/> for drawing and renderer information
        /// of a cell. Concrete implementations of this interface such as GridRenderStyleInfo
        /// or TreeRenderStyleInfo can add support for additional domain specific
        /// properties.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <returns></returns>
        public TreeRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex)
        {
            return RenderStyles.GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        /// <summary>
        /// Gets the render cell style for a cell. VirtualizingCellsControl solely relies
        /// on the <see cref="IRenderCellInfo"/> for drawing and renderer information
        /// of a cell. Concrete implementations of this interface such as GridRenderStyleInfo
        /// or TreeRenderStyleInfo can add support for additional domain specific
        /// properties.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="createDisposableObject">if set to <c>true</c> create disposable object that is not cached.</param>
        /// <returns></returns>
        public TreeRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex, bool createDisposableObject)
        {
            return RenderStyles.GetRenderStyleInfo(rowIndex, columnIndex, createDisposableObject);
        }

        /// <summary>
        /// Gets the render cell style for a cell. VirtualizingCellsControl solely relies
        /// on the <see cref="IRenderCellInfo"/> for drawing and renderer information
        /// of a cell. Concrete implementations of this interface such as GridRenderStyleInfo
        /// or TreeRenderStyleInfo can add support for additional domain specific
        /// properties.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="createDisposableObject">if set to <c>true</c> create disposable object that is not cached.</param>
        /// <returns></returns>
        public TreeRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex, bool createDisposableObject)
        {
            return RenderStyles.GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex, createDisposableObject);
        }


        #endregion

        #region PrepareRenderCell

        /// <summary>
        /// Raises the <see cref="PrepareRenderCell"/> event and optionally
        /// initializes the cells CellValue and CellRenderer.
        /// </summary>
        /// <param name="e">The <see cref="TreePrepareRenderCellEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPrepareRenderCell(TreePrepareRenderCellEventArgs e)
        {
            if (PrepareRenderCell != null)
                PrepareRenderCell(this, e);

            if (!e.Handled)
            {
                if (e.CellRowColumnIndex.RowIndex == 0)
                {
                    TreeColumn column = Model.Columns[e.CellRowColumnIndex.ColumnIndex];
                    e.Style.CellValue = column.Name;
                    e.Style.CellRenderer = treeColumnHeaderRenderer;
                }
                else if (e.CellRowColumnIndex.ColumnIndex == 0)
                {
                    //TreeNode node = Model.VisibleNodes[e.Cell.RowIndex];
                    //TreeColumn column = Model.Columns[0];

                    //e.Style.CellValue = node;
                    //e.Style.CellTemplateKey = "TreeNodeTemplate";
                    //e.Style.Description = node.Data.ToString();
                    e.Style.CellRenderer = treeNodeRenderer;
                    // treeNodeRenderer assigns node, template etc.
                }
                else
                {
                    TreeNode node = Model.VisibleNodes[e.CellRowColumnIndex.RowIndex];
                    TreeColumn column = Model.Columns[e.CellRowColumnIndex.ColumnIndex];
                    if (column.MappingName != null && node.Data != null)
                    {
                        PropertyDescriptor pd = Model.NodeItemProperties[column.MappingName];
                        if (pd != null && node.Data.GetType() == pd.ComponentType)
                        {
                            e.Style.CellValue = pd.GetValue(node.Data);
                            e.Style.CellValueType = pd.PropertyType;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Occurs just before a cell is arranged on the display in the 
        /// tree control and allows you to change the settings of the <see cref="TreeRenderStyleInfo"/>. 
        /// Changes made to the style's properties will only be made
        /// for rendering the style and will not be commited back to the tree node.
        /// </summary>  
        public event TreePrepareRenderCellEventHandler PrepareRenderCell;

        #endregion

        #region Clear Cached Cell Styles, Recalculate Scrollbars and Repaint.
        
        /// <summary>
        /// Refreshes the display for the tree clearing Cached Cell Styles, Recalculating Scrollbars and Repainting the 
        /// whole control.
        /// </summary>
        public void RefreshDisplay()
        {
            RenderStyles.Clear();
            RenderedCellVisuals.Invalidate();//new CellSpanInfoBase(0, 0, int.MaxValue, int.MaxValue));
            ArrangedCellUIElements.UnloadAll();
            ScrollRows.MarkDirty();
            ScrollColumns.MarkDirty();
            InvalidateVisual(true);
        }
        #endregion

        /// <summary>
        /// Implements handling for the PreviewMouseMove�event. When
        /// no mouse button is pressed and the mouse is over a cell it calls <see cref="DelayedCreateCellUIElements"/>.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (GridUtil.GetMouseButton(e) == null)
                DelayedCreateCellUIElements(PointToCellRowColumnIndex(e));

            base.OnPreviewMouseMove(e);
        }

        internal void RaisePrepareRenderCell(TreePrepareRenderCellEventArgs e)
        {
            OnPrepareRenderCell(e);
        }
    }


}
