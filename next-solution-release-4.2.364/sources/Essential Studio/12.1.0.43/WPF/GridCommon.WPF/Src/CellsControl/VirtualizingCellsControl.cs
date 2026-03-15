#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#define RectangleAsBorder
//#define TestDrawTextPerformance

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;

#if ENABLE_PARTIAL_TRUST
using System.Security.Permissions;
using System.Security;
#endif

#if SILVERLIGHT
using VirtualizingCellsControlChildFrame = Syncfusion.Windows.Controls.Scroll.ScrollControlChildFrame;
#endif

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// VirtualizingCellsControl is an abstract base class which can be used as a
    /// base class for any control with the purpose of displaying cells in scrollable
    /// rows and columns with built-in virtualization of visual elements inside the cells.
    /// <para/>
    /// VirtualizingCellsControl queries cell contents with the <see cref="GetRenderCellInfo"/>
    /// method which returns a <see cref="IRenderCellInfo"/> object. Each cell is associated
    /// with a <see cref="ICellRenderer"/> object which provides methods for measuring,
    /// arranging and drawing contents of a cell. The VirtualizingCellsControl implements
    /// the <see cref="OnArrangeContent"/> and <see cref="OnRender"/> methods. Within implementation
    /// of the OnArrangeContent method each cells UIElement children are placed on the controls area.
    /// A cell renderer can also be without any UIElement children and instead draw all its contents
    /// directly to the DrawingContext of the VirtualizingCellsControl when the <see cref="OnRender"/>
    /// method is executed. For a cell renderer it is also possible to do both: Arrange UIElements
    /// on the controls area and draw additional contents in its render area. There are also
    /// various optimization techniques that can be implemented with the renderer and are discussed
    /// in the <see cref="ICellRenderer"/> overview.
    /// <para/>
    /// VirtualizingCellsControl provides a few standard features that can be utilized
    /// by derived grid or tree controls such as the "Covered Cell" feature (see
    /// <see cref="CoveredCellsProvider"/>), the "SpanBackground" (
    /// <see cref="CellSpanBackgroundsProvider"/>)
    /// feature, support for MouseControllers (<see cref="MouseControllerDispatcher"/>), create an
    /// UIElement on demand when hovering mouse over cell, cell borders and methods that 
    /// convert from display coordinates to cell coordinates and vice versa.
    /// <para/>
    /// The virtualization of UIElement children of cell renderers is implemented in 
    /// the arrange cells code. At the time a cell is placed the cell renderer is 
    /// called to create and intialize the UIElement children. When a cell is scrolled
    /// out of view the cell renderer is called to unload the UIElement children. A cell
    /// renderer can decide whether to unload a UIElement, keep it alive or move
    /// it to a recycle bin and reuse it later. The main logic of this code is implemented
    /// by the <see cref="VirtualizingCellRendererBase{T}"/> class which contains more detailed
    /// discussion about this feature.
    /// </summary>

#if ENABLE_PARTIAL_TRUST
    [SecuritySafeCritical]    
#endif
    public abstract class VirtualizingCellsControl : ScrollAxisControl
    {
        #region Fields
#if !SILVERLIGHT
        // MouseController
        CellMouseControllerDispatcher mouseControllerDispatcher = null;

        RenderedCellsManager renderedCells;

        // DrawingVisuals
        bool useDrawingVisualForCells = true;
        bool isRenderCellBackgroundPhase = false;
        bool isInOnRender = false;

        DrawingVisual foregroundFrameCellsVisual = new NoHitTestDrawingVisual();        
        DrawingVisual backgroundFrameBorderVisual = new NoHitTestDrawingVisual();
        DrawingVisual cellBackgroundDrawingVisuals = new NoHitTestDrawingVisual();
        DrawingVisual cellBackgroundRenderStyleDrawingVisuals = new NoHitTestDrawingVisual();
        /// <summary>
        /// Contains the range details of Overlapping cells
        /// </summary>
        protected Dictionary<RowColumnIndex, int> floatcellran = new Dictionary<RowColumnIndex, int>();

        bool clearVisualsCacheWhenUnloaded = false;
        VisualContainer backgroundFrameBorders;
#else
        Panel cellBackgrounds = new VisualContainer("CellBackgrounds");
        Panel backgroundFrameBorders = new VisualContainer("CellBorders");
#endif

        // Virtualized UI Elements
        ArrangedCellUIElementsManager arrangedCellUIElements;
       
        bool needRenderBorders = true; // set in ArrangeBorders
        bool needRenderBackgrounds = true; // set in ArrangeCombinedCellBackgrounds
        bool needRenderStyleBackgrounds = true; // set this when PrepareRenderCellInfo can change background.

        // ArrangeCombinedCellBackgrounds
        List<VisibleCombinedCellBackgroundInfo> combinedCellBackgroundsList = new List<VisibleCombinedCellBackgroundInfo>();
        List<List<int>> combinedCellBackgroundIdsTable = new List<List<int>>();
        int combinedCellBackgroundArrangeId = 1;
        bool backgroundsDirty = true;
        bool bordersDirty = true;

        // Covered Cells
        ICoveredCellsProvider coveredCellsProvider;
        VisibleCellSpanLayout<VisibleCoveredCellInfo> coveredCellsLayout;
        int visibleCoveredCellsArrangeId = 0;

        //Overlapping Cells
        IOverlappingCellProvider overlappingCellsProvider;
        VisibleCellSpanLayout<VisibleOverlappingCellInfo> overlappingCellsLayout;
        int visibleOverlappingCellsArrangeId = 0;
        

        // CellSpanBackgrounds
        ICellSpanBackgroundsProvider cellSpanBackgroundsProvider;
        List<List<VisibleCellSpanBackgroundInfo>> framesListWithBackgoundSpans;

        // Helper
        bool isInArrage;


        #endregion

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets whether the control should clear out cache with visuals,
        /// arrange ui elemnts and cell styles when the control is unloaded. The
        /// default setting is false.
        /// </summary>
        [DefaultValue(false)]
        public bool ClearVisualsCacheWhenUnloaded
        {
            get { return clearVisualsCacheWhenUnloaded; }
            set { clearVisualsCacheWhenUnloaded = value; }
        }

        /// <summary>
        /// This virtual method is called from the <see cref="FrameworkElement.Unloaded"/> event handler.<para/>
        /// Override this method to clear cached settings (e.g. rendered styles, visibility of rows) when the
        /// control was unloaded. Do not unwire events here since a control can be unloaded and loaded
        /// multiple times during its lifetime.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected override void OnUnloaded(RoutedEventArgs e)
        {
            if (ClearVisualsCacheWhenUnloaded)
            {
                if (RenderedCellVisuals !=null)
                RenderedCellVisuals.Invalidate();
                if (ArrangedCellUIElements !=null)
                ArrangedCellUIElements.UnloadAll();
                if (this.combinedCellBackgroundsList != null)
                    combinedCellBackgroundsList.Clear();
                if (this.combinedCellBackgroundIdsTable != null)
                    combinedCellBackgroundIdsTable.Clear();
            }

            base.OnUnloaded(e);
        }
#endif

        /// <summary>
        /// Gets or sets whether border lines have to be rounded off.
        /// </summary>
        [DefaultValue(false)]
        public bool AllowBorderRounding
        {
            get;
            set;
        }

        #region Dependency Properties

        #region CellsControlProperty

        /// <summary>
        /// Returns the <see cref="VirtualizingCellsControl"/> of an UIElement inside a cell. When the editor
        /// inside a cell has children and you query this for a child it will query the top-most 
        /// parent element of the cell renderer for the value of the property.
        /// </summary>
        public static readonly DependencyProperty CellsControlProperty = DependencyProperty.Register(
        "CellsControl", typeof(VirtualizingCellsControl), typeof(VirtualizingCellsControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the <see cref="CellsControlProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static VirtualizingCellsControl GetCellsControl(DependencyObject dpo)
        {
            return (VirtualizingCellsControl)GridUtil.GetValueInherited(dpo, CellsControlProperty, null);
        }

        public static VirtualizingCellsControl GetCellRendererParentControl(DependencyObject dpo)
        {
            return (VirtualizingCellsControl)GridUtil.GetValueInherited(dpo, CellsControlProperty, CellRendererProperty, null);
        }

        /// <summary>
        /// Sets the <see cref="CellsControlProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetCellsControl(DependencyObject dpo, VirtualizingCellsControl value)
        {
            dpo.SetValue(CellsControlProperty, value);
        }
        #endregion
        #region CellUIElementProperty
        /// <summary>
        /// Returns the top-most parent element of the cell renderer When the editor
        /// inside a cell has children and you query this attached property for a child it will query the top-most 
        /// parent element of the cell renderer for the value of the property.
        /// </summary>
        public static readonly DependencyProperty CellUIElementProperty = DependencyProperty.Register(
            "CellUIElement", typeof(UIElement), typeof(VirtualizingCellsControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the <see cref="CellUIElementProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static UIElement GetCellUIElement(DependencyObject dpo)
        {
            return (UIElement)GridUtil.GetValueInherited(dpo, CellUIElementProperty, null);
        }

        /// <summary>
        /// Sets the <see cref="CellUIElementProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetCellUIElement(DependencyObject dpo, UIElement value)
        {
            dpo.SetValue(CellUIElementProperty, value);
        }
        #endregion
        #region ArrangeCellArgsEventProperty
        public static readonly DependencyProperty ArrangeCellArgsProperty = DependencyProperty.Register(
            "ArrangeCellArgsEvent", typeof(ArrangeCellArgs), typeof(VirtualizingCellsControl), new FrameworkPropertyMetadata(null));

        public static ArrangeCellArgs GetArrangeCellArgs(DependencyObject dpo)
        {
            return (ArrangeCellArgs)dpo.GetValue(ArrangeCellArgsProperty);
        }

        public static void SetArrangeCellArgs(DependencyObject dpo, ArrangeCellArgs value)
        {
            dpo.SetValue(ArrangeCellArgsProperty, value);
        }
        #endregion
        #region CellRowColumnIndex
        /// <summary>
        /// Returns the <see cref="RowColumnIndex"/> of an UIElement inside a cell. When the editor
        /// inside a cell has children and you query this attached property for a child it will query the top-most 
        /// parent element of the cell renderer for the value of the property.
        /// </summary>
        public static readonly DependencyProperty CellRowColumnIndexProperty = DependencyProperty.Register(
            "CellRowColumnIndex", typeof(RowColumnIndex), typeof(VirtualizingCellsControl), new FrameworkPropertyMetadata(RowColumnIndex.Empty));

        /// <summary>
        /// Gets the <see cref="CellRowColumnIndexProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static RowColumnIndex GetCellRowColumnIndex(DependencyObject dpo)
        {
            return (RowColumnIndex)GridUtil.GetValueInherited(dpo, CellRowColumnIndexProperty, RowColumnIndex.Empty);
        }

        /// <summary>
        /// Sets the <see cref="CellRowColumnIndexProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetCellRowColumnIndex(DependencyObject dpo, RowColumnIndex value)
        {
            dpo.SetValue(CellRowColumnIndexProperty, value);
        }
        #endregion
        #region RenderCellInfoProperty
        /// <summary>
        /// Returns the <see cref="IRenderCellInfo"/> of an UIElement inside a cell. When the editor
        /// inside a cell has children and you query this attached property for a child it will query the top-most 
        /// parent element of the cell renderer for the value of the property.
        /// </summary>
        public static readonly DependencyProperty RenderCellInfoProperty = DependencyProperty.Register(
            "RenderCellInfo", typeof(IRenderCellInfo), typeof(VirtualizingCellsControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the <see cref="RenderCellInfoProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static IRenderCellInfo GetRenderCellInfo(DependencyObject dpo)
        {
            return (IRenderCellInfo)GridUtil.GetValueInherited(dpo, RenderCellInfoProperty, null);
        }

        /// <summary>
        /// Sets the <see cref="RenderCellInfoProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetRenderCellInfo(DependencyObject dpo, IRenderCellInfo value)
        {
            dpo.SetValue(RenderCellInfoProperty, value);
        }
        #endregion
        #region CellRendererProperty
        /// <summary>
        /// Returns the cell renderer of an UIElement inside a cell. When the editor
        /// inside a cell has children and you query this attached property for a child it will query the top-most 
        /// parent element of the cell renderer for the value of the property.
        /// </summary>
        public static readonly DependencyProperty CellRendererProperty = DependencyProperty.Register(
            "CellRenderer", typeof(ICellRenderer), typeof(VirtualizingCellsControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the <see cref="CellRendererProperty"/> dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static ICellRenderer GetCellRenderer(DependencyObject dpo)
        {
            return (ICellRenderer)GridUtil.GetValueInherited(dpo, CellRendererProperty, null);
        }

        /// <summary>
        /// Sets the <see cref="CellRendererProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetCellRenderer(DependencyObject dpo, ICellRenderer value)
        {
            dpo.SetValue(CellRendererProperty, value);
        }
        #endregion
        #region HasFocusWithinProperty
#if SILVERLIGHT
        /// <summary>
        /// The value of this property represents the distance between the left side of an element 
        /// and the left side of its parent Canvas. When the editor
        /// inside a cell has children and you query this attached property for a child it will query the top-most 
        /// parent element of the cell renderer for the value of the property.
        /// </summary>
        public static readonly DependencyProperty HasFocusWithinProperty = DependencyProperty.RegisterAttached(
            "HasFocusWithin", typeof(bool), typeof(VirtualizingCellsControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets the <see cref="HasFocusWithinProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static bool GetHasFocusWithin(DependencyObject dpo)
        {
            return (bool) GridUtil.GetValueInherited(dpo, HasFocusWithinProperty, false);
        }

        /// <summary>
        /// Sets the <see cref="HasFocusWithinProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetHasFocusWithin(DependencyObject dpo, bool value)
        {
            dpo.SetValue(HasFocusWithinProperty, value);
        }
#else
        public static bool GetHasFocusWithin(UIElement el)
        {
            return el.IsKeyboardFocusWithin;
        }
#endif
        #endregion

        public bool ShowGridLines
        {
            get { return (bool)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(VirtualizingCellsControl), new PropertyMetadata(false, OnShowGridLinesChanged));

        private static void OnShowGridLinesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VirtualizingCellsControl cellsControl = obj as VirtualizingCellsControl;
            if (cellsControl != null)
                cellsControl.OnShowGridLinesChanged(args);
        }

        protected virtual void OnShowGridLinesChanged(DependencyPropertyChangedEventArgs args)
        {
        }
        
        #endregion

        #region Ctor, Unload
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCellsControl"/> class.
        /// </summary>
        public VirtualizingCellsControl()
        {
            arrangedCellUIElements = new ArrangedCellUIElementsManager(this);

#if !SILVERLIGHT
            renderedCells = new RenderedCellsManager(this.InnerFrame.Children);

            BackgroundFrame.Children.Add(cellBackgroundDrawingVisuals);
            BackgroundFrame.Children.Add(cellBackgroundRenderStyleDrawingVisuals);
            ForegroundFrame.Children.Add(foregroundFrameCellsVisual);            
            BackgroundFrame.Children.Add(backgroundFrameBorderVisual);
            backgroundFrameBorders = new VisualContainer("CellBorders");
            BackgroundFrame.Children.Add(backgroundFrameBorders);
#else
            BackgroundFrame.Children.Add(cellBackgrounds);
            BackgroundFrame.Children.Add(backgroundFrameBorders);
#endif
            VisualContainer.SetWantsMouseInput(this, true);
        }

        /// <summary>
        /// Creates the scroll control child frame object.
        /// </summary>
        /// <returns></returns>
        protected override ScrollControlChildFrame CreateScrollControlChildFrame()
        {
            ScrollControlChildFrame cf = new VirtualizingCellsControlChildFrame();
            VirtualizingCellsControl.SetCellsControl(cf, this);
            VisualContainer.SetWantsMouseInput(cf, true);
            return cf;
        }

        protected override void SetFrameProperties(VisualContainer parent, ScrollControlChildFrame child)
        {
            if (parent.Name.Equals("InnerFrame") && this.IsInOnRender)
            {
                var virtualizingFrame = child as VirtualizingCellsControlChildFrame;
                if (virtualizingFrame != null && !virtualizingFrame.IsPrepareRenderCellIntialized)
                    virtualizingFrame.RenderedCells.PrepareRenderCells();
            }
        }


        #endregion

        #region CoveredCellsProvider, CellSpanBackgroundsProvider

        /// <summary>
        /// Gets or sets the covered cells provider. A covered cell is a cell
        /// that spans multiple neighbouring cells.
        /// </summary>
        /// <value>The covered cells provider.</value>
        public ICoveredCellsProvider CoveredCellsProvider
        {
            get { return coveredCellsProvider; }
            set
            {
                if (coveredCellsProvider != value)
                {
                    coveredCellsLayout = null;
                    coveredCellsProvider = value;
                    InvalidateVisual(true);
                }
            }
        }

        public IOverlappingCellProvider OverlappingCellsProvider
        {
            get { return overlappingCellsProvider; }
            set
            {
                if (overlappingCellsProvider != value)
                {
                    overlappingCellsLayout = null;
                    overlappingCellsProvider = value;
                    InvalidateVisual(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the cell span backgrounds provider. A spanned background
        /// allows one cell to draw its background across multiple neighbouring
        /// cells. The neighbouring cells are still individual cells with their
        /// own editor but all share the cell backgound. You can example attach
        /// a image to one cell and draw it across multiple cells.
        /// </summary>
        /// <value>The cell span backgrounds provider.</value>
        public ICellSpanBackgroundsProvider CellSpanBackgroundsProvider
        {
            get { return cellSpanBackgroundsProvider; }
            set
            {
                if (cellSpanBackgroundsProvider != value)
                {
                    cellSpanBackgroundsProvider = value;
                    framesListWithBackgoundSpans = null;
                    InvalidateVisual(true);
                }
            }
        }

        #endregion

        #region MouseControllerDispatcher
        /// <summary>
        /// MouseControllerDispatcher coordinates mouse events among competing mouse controllers. Based on
        /// the position of the mouse and context of the control every registered controller's HitTest method
        /// is called to determine the best controller for the following mouse action. This controller will then
        /// receive mouse events.
        /// </summary>
        /// <remarks>
        /// See <see cref="CellMouseControllerDispatcher"/> for more information.
        /// </remarks>
        public CellMouseControllerDispatcher MouseControllerDispatcher
        {
            get
            {
                // Will get instantiated on demand when user tries to add IMouseControllers.
                if (mouseControllerDispatcher == null)
                {
                    MouseControllerDispatcher = new CellMouseControllerDispatcher(this);
                }
                return mouseControllerDispatcher;
            }
            internal set
            {
                if (this.mouseControllerDispatcher != value)
                {
                    if (this.mouseControllerDispatcher != null)
                        this.MouseEventListeners.Remove(mouseControllerDispatcher);
                    this.mouseControllerDispatcher = value;
                    if (this.mouseControllerDispatcher != null)
                        this.MouseEventListeners.Add(mouseControllerDispatcher);
                }
            }
        }
        #endregion

        #region Keyboard and ScrollInDirection

        /// <summary>
        /// Implements handling for the PreviewKeyDown event. When
        /// a user pressed Escape it will call CancelMode of
        /// the <see cref="MouseControllerDispatcher"/>.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Key == Key.Escape && MouseControllerDispatcher.ActiveController != null)
                    MouseControllerDispatcher.CancelMode();
            }

            base.OnPreviewKeyDown(e);
        }

        /// <summary>
        /// Shoulds the cells control handle the key down scrolling in a KeyDown event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        protected virtual bool ShouldCellsControlHandleKeyDownScrolling(KeyEventArgs e)
        {
            return true;
        }

        /// <summary>
        /// Implements handling for the KeyDown event. When
        /// a user presses scroll or arrow keys this method will scroll the control.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled && ShouldCellsControlHandleKeyDownScrolling(e))
                ScrollInDirection(e);
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Scrolls the grid in arrow key direction. This usually is called from the KeyDown event handler.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        public void ScrollInDirection(KeyEventArgs e)
        {
            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.None)
            {
                bool isRightToLeft = base.FlowDirection == FlowDirection.RightToLeft;
                switch (e.Key)
                {
                    case Key.Prior:
                        PageUp();
                        e.Handled = true;
                        return;

                    case Key.Next:
                        PageDown();
                        e.Handled = true;
                        return;

                    case Key.End:
                        if (!isControlKey)
                        {
                            ScrollToRightEnd();
                        }
                        else
                        {
                            ScrollToBottom();
                        }
                        e.Handled = true;
                        return;

                    case Key.Home:
                        if (!isControlKey)
                        {
                            ScrollToLeftEnd();
                        }
                        else
                        {
                            ScrollToTop();
                        }
                        e.Handled = true;
                        return;

                    case Key.Left:
                        if (!isRightToLeft)
                        {
                            LineLeft();
                            break;
                        }
                        LineRight();
                        break;

                    case Key.Up:
                        LineUp();
                        e.Handled = true;
                        return;

                    case Key.Right:
                        if (!isRightToLeft)
                        {
                            LineRight();
                        }
                        else
                        {
                            LineLeft();
                        }
                        e.Handled = true;
                        return;

                    case Key.Down:
                        LineDown();
                        e.Handled = true;
                        return;

                    default:
                        return;
                }
            }
        }

        #endregion

        #region Render to DrawingContext

        /// <summary>
        /// Gets or sets a value indicating whether the VirtualizingCellsControl
        /// should render cells using a DrawingVisual for each cell. If false
        /// the control will render all cells to the same drawing context instead 
        /// (which will be slower).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if  the VirtualizingCellsControl
        /// should render cells using a DrawingVisual for each cell; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRenderCellDrawingVisuals
        {
            get { return useDrawingVisualForCells; }
            set { useDrawingVisualForCells = value; }
        }

        /// <summary>
        /// Gets the manager object for DrawingVisuals of visible cells.
        /// </summary>
        /// <value>The rendered cell visuals.</value>
        public RenderedCellsManager RenderedCellVisuals
        {
            get { return renderedCells; }
        }

        public bool IsInOnRender
        {
            get { return isInOnRender; }
        }
        
        //Whenever the horizontal scrollbar value changed the DataTemplate cells are refreshed
        //Earlier DataTempalte cells are refreshed on OnRender() method this leads to frequent refresh of DataTemplate cells. Now this was prevented
        //protected override void OnHScrollBarValueChanged(object sender, EventArgs e)
        //{
        //    ArrangedCellUIElements.RefreshDirtyCellUIElementsContent();
        //    base.OnHScrollBarValueChanged(sender, e);
        //}

        /// <summary>
        /// Implements the render logic for the cells control. The method first renders
        /// cell backgrounds, then the individual cells for each cell calling <see cref="OnRenderCell"/>,
        /// then covered cells and finally the cell borders.<para/>
        /// Cell Backgrounds are rendered to the <see cref="ScrollControl.BackgroundFrame"/> behind cells.
        /// Cell Borders are rendered to the <see cref="ScrollControl.ForegroundFrame"/> in front of cells.
        /// Cells are rendered row by row to the ForegroundFrame but they will be drawn
        /// in code before the cell borders are rendered and thus appear behind cell borders. The 
        /// rendering of cells also places the rendered contents of a cell in front of the contents 
        /// of placed UIElement children of cell renderers.<para/>
        /// The UIElement children of cellrenderers are placed within the frames provided by 
        /// the <see cref="ScrollControl"/> base class. (see <see cref="OnArrangeContent"/>.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        protected override void OnRender(DrawingContext dc)
        {
            //Console.WriteLine("OnRender");
            isInOnRender = true;
            if (useDrawingVisualForCells)
            {
                // dc will draw be behind any frame. 
                foregroundFrameCellsVisual.Clip = null;

#if !TestDrawTextPerformance
                bool isdirtybackgroundflag = backgroundsDirty;
                RenderBackgrounds();

                // Draw cell backgrounds (cells with RenderCellInfo.Background != ModelCellInfo.Background)
                this.isRenderCellBackgroundPhase = true;

                if (ShouldRenderStyleBackgrounds() || isdirtybackgroundflag)
                {
                    using (DrawingContext dc0 = cellBackgroundRenderStyleDrawingVisuals.RenderOpen())
                    {
                        RenderCells(dc0); // OnRenderCell will draw indidividual cells backgroudn to backgroundDrawingContext.
                        RenderCoveredCells(dc0);
                        RenderOverlappingCells(dc0);
                    }
                    //needRenderStyleBackgrounds = false;
                }

                // Draw cell contents - no dc given, each cell will have a DrawingVisual and draw into its dc.
                this.isRenderCellBackgroundPhase = false;

                RenderedCellVisuals.PrepareRenderCells();
#endif
                RenderCells(null);

#if !TestDrawTextPerformance
                RenderCoveredCells(null);

                RenderOverlappingCells(null);

                RenderedCellVisuals.ConcludeRenderCells();

                RenderCellBorders();
#endif
            }
            else
            {
                RenderBackgrounds();

                this.isRenderCellBackgroundPhase = true;
                if (ShouldRenderStyleBackgrounds())
                    RenderCells();

                this.isRenderCellBackgroundPhase = false;
                RenderCells();

                RenderCellBorders();
            }
            
            //Commented this line. Since it leads to  frequent refresh of DataTemplate cells.
            //This code previously commented.Beacuse of Scrolling Performance issue.
            //If this ArrangedCellUIElements.RefreshDirtyCellUIElementsContent();  is commented then it leads to some breaking issues. So, again it was uncommented
            ArrangedCellUIElements.RefreshDirtyCellUIElementsContent();     
                                                                          
            base.OnRender(dc);
            isInOnRender = false;
        }

        protected virtual bool ShouldRenderStyleBackgrounds()
        {
            return needRenderStyleBackgrounds;
        }

        void RenderBackgrounds()
        {
            if (backgroundsDirty)
            {
                // InvalidateCellBackground was called and there has been no
                // OnArrangeContent call since then (probably because Invalidate(false) was called
                // which will skip OnArrangeContent in layout pass).
                ArrangeCellSpanBackgrounds(this.RenderSize);

                // Calculate Cells Background - combine cells with same brush into one draw operation.
                ArrangeCombinedCellBackgrounds(RenderSize);
            }

            if (!needRenderBackgrounds) return;
            needRenderBackgrounds = false;
            using (DrawingContext dc0 = cellBackgroundDrawingVisuals.RenderOpen())
            {
                RenderBackgrounds(dc0);
            }
        }

        void RenderCells()
        {
            DrawingVisual dv;
            if (isRenderCellBackgroundPhase)
                dv = cellBackgroundRenderStyleDrawingVisuals;
            else
                dv = foregroundFrameCellsVisual;

            dv.Clip = null;
            dv.Children.Clear();
            using (DrawingContext dc = dv.RenderOpen())
            {
                RenderCells(dc);
                RenderCoveredCells(dc);
                RenderOverlappingCells(dc);
            }
        }

        void RenderCellBorders()
        {
            if (!needRenderBorders) return;
            needRenderBorders = false;
            backgroundFrameBorderVisual.Clip = null;
            backgroundFrameBorderVisual.Children.Clear();
            using (DrawingContext dc2 = backgroundFrameBorderVisual.RenderOpen())
            {
                RenderCellBorders(dc2);               
            }
        }

        /// <summary>
        /// Renders the backgrounds of cells including background of normal cells, spanned background
        /// cells and covered cells.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        protected virtual void RenderBackgrounds(DrawingContext dc)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("VirtualizingCellsControl.RenderBackgrounds"))
            { 
#endif
            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();
            foreach (VisibleCombinedCellBackgroundInfo s in combinedCellBackgroundsList)
            {
                VisibleLineInfo startRow = visibleRows[s.top];
                VisibleLineInfo endRow = visibleRows[s.bottom];
                VisibleLineInfo startColumn = visibleColumns[s.left];
                VisibleLineInfo endColumn = visibleColumns[s.right];

                if (endColumn.ClippedCorner > startColumn.ClippedOrigin
                    && endRow.ClippedCorner > startRow.ClippedOrigin)
                {
                    Rect clippedBounds = GridUtil.FromLTRB(startColumn.ClippedOrigin, startRow.ClippedOrigin, endColumn.ClippedCorner, endRow.ClippedCorner);
                    Rect exactBounds = GridUtil.FromLTRB(startColumn.Origin, startRow.Origin, endColumn.Corner, endRow.Corner); ;
#if true
                    if (s.background is SolidColorBrush || clippedBounds == exactBounds)
                    {
                         if (this.UseGuidelineSetToRenderBackground)
                       {
                            var guidelines = new GuidelineSet();
                            guidelines.GuidelinesX.Add(clippedBounds.Left);
                            guidelines.GuidelinesX.Add(clippedBounds.Right);
                            guidelines.GuidelinesY.Add(clippedBounds.Top);
                            guidelines.GuidelinesY.Add(clippedBounds.Bottom);
                            dc.PushGuidelineSet(guidelines);
                        }
                        dc.DrawRectangle(s.background, null, clippedBounds);

                        //Console.WriteLine("dc.DrawRectangle({0});", clippedBounds);
                    }
                    else
                    {
                        GridUtil.PushClip(dc, clippedBounds);
                        dc.DrawRectangle(s.background, null, exactBounds);
                        //Console.WriteLine("dc.DrawRectangle({0});", exactBounds);
                        dc.Pop();
                    }
#else
                // clip out cells for which InvalidateCellBackground was called.
                Geometry clipGeometry = null;

                for (int r = s.top; r <= s.bottom; r++)
                {
                    for (int c = s.left; c <= s.right; c++)
                    {
                        if (combinedCellBackgroundIdsTable[r][c] == -1)
                        {
                            VisibleLineInfo visibleRow = visibleRows[r];
                            VisibleLineInfo visibleColumn = visibleColumns[c];
                            if (visibleRow != null && visibleColumn != null)
                            {
                                if (clipGeometry == null)
                                    clipGeometry = new RectangleGeometry(rect);
                                Rect cellRect = new Rect(visibleColumn.ClippedOrigin, visibleRow.ClippedOrigin, visibleColumn.ClippedSize, visibleRow.ClippedSize);
                                clipGeometry = Geometry.Combine(clipGeometry, new RectangleGeometry(cellRect), GeometryCombineMode.Exclude, null);
                            }
                        }
                    }
                }
                if (clipGeometry != null)
                    dc.PushClip(clipGeometry);
                dc.DrawRectangle(s.background, null, rect);
                if (clipGeometry != null)
                    dc.Pop();
#endif
                }
            }

            // Draw one background accross multiple cells (specified by user)
            if (framesListWithBackgoundSpans != null)
            {
                foreach (List<VisibleCellSpanBackgroundInfo> backgroundSpans in framesListWithBackgoundSpans)
                {
                    foreach (VisibleCellSpanBackgroundInfo bg in backgroundSpans)
                    {
                        VisibleLineInfo visibleRow = ScrollRows.GetVisibleLines()[bg.Top];
                        VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLines()[bg.Left];
                        Rect cellRect = bg.ExactBounds;

                        if (bg.ClippedBounds != cellRect)
                        {
                            Rect clipRect = GetClipRect((ScrollAxisRegion)bg.RowSection, (ScrollAxisRegion)bg.ColumnSection);
                            PushClip(dc, clipRect);
                            dc.DrawRectangle(bg.CellSpanBackground.Background, bg.CellSpanBackground.Border, cellRect);
                            dc.Pop();
                        }
                        else
                            dc.DrawRectangle(bg.CellSpanBackground.Background, bg.CellSpanBackground.Border, cellRect);
                    }
                }
            }
#if MEASURETIME
            }
#endif
        }

        #region RenderCells

        /// <summary>
        /// Renders the cells row by row. For each row the virtual <see cref="RenderRow"/> method is 
        /// called which then calls <see cref="OnRenderCell"/> for each cell. OnRenderCell gets
        /// the <see cref="ICellRenderer"/> for a cell and calls its <see cref="ICellRenderer.Render"/>
        /// method.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        protected virtual void RenderCells(DrawingContext dc)
        {
            Point corner = new Point(ScrollColumns.ViewCorner, ScrollRows.ViewCorner);
            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            double viewWidth = ScrollColumns.ViewSize;
            visibleCoveredCellsArrangeId++;
            visibleOverlappingCellsArrangeId++;
            
            bool hasOriginMargin = Margin.Top > 0;
            bool hasCornerMargin = Margin.Bottom > 0;
            try
            {
                ScrollRows.FreezeVisibleLines();
                // Rows with cells
                foreach (VisibleLineInfo visibleRow in visibleRows)
                {
                    if (visibleRow.Size == 0)
                        continue;

                    // dc can be null if useDrawingVisualForCells is set.
                    bool clip = (isRenderCellBackgroundPhase || !this.useDrawingVisualForCells) && dc != null && visibleRow.IsClippedBodyAny(hasOriginMargin, hasCornerMargin);
                    if (clip) PushClip(dc, new Rect(0, visibleRow.ClippedOrigin, viewWidth, visibleRow.ClippedSize));
                    RenderRow(dc, visibleRow, corner);
                    if (clip) dc.Pop();
                }
            }
            finally
            {
                ScrollRows.UnfreezeVisibleLines();
            }
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
        protected virtual void RenderRow(DrawingContext dc, VisibleLineInfo visibleRow, Point corner)
        {
            int rowIndex = visibleRow.LineIndex;

            Rect cellRect = new Rect(0, visibleRow.Origin, ScrollColumns.ViewSize, visibleRow.Size);

            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();
            try
            {
                ScrollColumns.FreezeVisibleLines();
                foreach (VisibleLineInfo visibleColumn in visibleColumns)
                {
                    // covered cells will be rendered separately in RenderCoveredCells
                    if (GetVisibleCoveredCell(visibleRow, visibleColumn) != null)
                        continue;

                    if (GetVisibleOverlappingCell(visibleRow, visibleColumn) != null)
                        continue;

                    int columnIndex = visibleColumn.LineIndex;
                    cellRect.X = visibleColumn.Origin;
                    cellRect.Width = visibleColumn.Size;

                    IRenderCellInfo renderCellInfo = InternalGetCellInfo(visibleRow, visibleColumn);
                    RenderCellArgs rca = new RenderCellArgs(this, visibleRow, visibleColumn, cellRect, renderCellInfo);
                    RenderCell(dc, rca);
                }
            }
            finally
            {
                ScrollColumns.UnfreezeVisibleLines();
            }
        }

        protected void RenderCell(DrawingContext dc, RenderCellArgs rca)
        {
            VisibleLineInfo visibleRow = rca.VisibleRow;
            VisibleLineInfo visibleColumn = rca.VisibleColumn;

            bool hasOriginMargin = Margin.Left > 0;
            bool hasCornerMargin = Margin.Right > 0;

            if (isRenderCellBackgroundPhase)
            {
                var rect = rca.CellClipRect;
                if (this.AllowBorderRounding && visibleColumn.IsClipped)
                {
                    rect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
                    rect.Inflate(0, .5);
                }
                bool isClip = visibleColumn.IsClippedBodyAny(hasOriginMargin, hasCornerMargin);
                if (isClip) PushClip(dc, rect);
                OnRenderCellBackground(dc, rca);
                if (isClip) dc.Pop();
            }
            else
            {
                if (useDrawingVisualForCells)
                {
                    VirtualizingCellsControlChildFrame canvas = (VirtualizingCellsControlChildFrame)GetChildFrame(rca, this.InnerFrame);
                    DrawingVisual dv;
                    if (canvas.RenderedCells.TransformOrCreateDrawingVisual(rca, out dv))
                    {
                        using (DrawingContext dc2 = dv.RenderOpen())
                        {
                            OnRenderCell(dc2, rca);
                        }
                    }
                }
                else
                {
                    /* SH 12/8/11
                     * I did some minor tweaking with clipping of rows and colums when rendering the cells. 
                     * Previously the last column was clipped when only partially visible. But this is not
                     * really needed to call DrawingContext.PushClip since it is drawing out of bounds anyway,
                     * so I got rid of this extra clipping call. I added VisibleLineInfo.IsClippedBody in
                     * VisibleLineInfo to be able to check for the refined criteria when clipping is needed.
                    */
                    bool isClip = visibleColumn.IsClippedBodyAny(hasOriginMargin, hasCornerMargin);
                    if (isClip) PushClip(dc, rca.CellClipRect);
                    OnRenderCell(dc, rca);
                    if (isClip) dc.Pop(); 
                }
            }
        }

        /// <summary>
        /// Called to render a cell (both covered and non-covered). 
        /// The default implementation of this method gets
        /// the <see cref="ICellRenderer"/> for a cell and calls its <see cref="ICellRenderer.Render"/>
        /// method. If the cells background was not drawn yet the method gets the background 
        /// with <see cref="GetCellBackground"/> and draws it to the drawing context. The method
        /// also adjust the <see cref="CellArgs.CellRect"/> and subtracts the border margins
        /// from the rectangle.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rca">The cell layout information.</param>
        protected virtual void OnRenderCell(DrawingContext dc, RenderCellArgs rca)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("VirtualizingCellsControl.OnRenderCell"))
            {
#endif
            if (rca.CellRect.IsEmpty)
                return;

            rca.CellRect = rca.SubtractBorderMargins(rca.CellRect, rca.CellInfo.GetBorderMargins());

            if (rca.CellRect.IsEmpty)
                return;

            ICellRenderer renderer = GetCellRenderer(rca.CellInfo);
            if (renderer != null)
                renderer.Render(dc, rca);

#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Called to render the background of a cell to a drawing context. The method
        /// first checks if the background of the cell is different from the
        /// default cell background and only then draws the background for all cell in a covered cell range.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="ccSpan">CoveredCellInfo.</param>
        /// <param name="rca">The rca.</param>
        protected virtual void OnRenderCellBackground(DrawingContext dc, VisibleCoveredCellInfo ccSpan, RenderCellArgs rca)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("VirtualizingCellsControl.OnRenderCellBackground"))
            {
#endif
            Brush background = null;
            bool backgroundDrawn = false;

            if (ccSpan != null)
            {
                for (int i = ccSpan.Left; i <= ccSpan.Right; i++)
                    for (int j = ccSpan.Top; j <= ccSpan.Bottom; j++)
                    {
                        NeedsToDrawBackground(rca.CellInfo, j, i, out background, out backgroundDrawn);
                        if (!backgroundDrawn)
                        {
                            rca.CellRect = rca.SubtractBorderMargins(rca.CellRect, rca.CellInfo.GetPadding());
                            dc.DrawRectangle(background, null, rca.CellRect);
                        }
                    }
            }
#if MEASURETIME
            }
#endif
        }

        protected virtual void OnRenderCellBackground(DrawingContext dc, VisibleOverlappingCellInfo icSpan, RenderCellArgs rca)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("VirtualizingCellsControl.OnRenderCellBackground"))
            {
#endif
            Brush background = null;
            bool backgroundDrawn = false;

            if (icSpan != null)
            {
                for (int i = icSpan.Left; i <= icSpan.Right; i++)
                    for (int j = icSpan.Top; j <= icSpan.Bottom; j++)
                    {
                        NeedsToDrawBackground(rca.CellInfo, j, i, out background, out backgroundDrawn);
                        if (!backgroundDrawn)
                        {
                            rca.CellRect = rca.SubtractBorderMargins(rca.CellRect, rca.CellInfo.GetPadding());
                            dc.DrawRectangle(background, null, rca.CellRect);
                        }
                    }
            }
#if MEASURETIME
            }
#endif
        }

        protected virtual void OnRenderCellBackground(DrawingContext dc, RenderCellArgs rca)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("VirtualizingCellsControl.OnRenderCellBackground"))
            {
#endif
            Brush background = null;
            bool backgroundDrawn = false;
            NeedsToDrawBackground(rca, out background, out backgroundDrawn);
            if (!backgroundDrawn)
            {
                rca.CellRect = rca.SubtractBorderMargins(rca.CellRect, rca.CellInfo.GetPadding());
                var rect = rca.CellRect;
                if (this.AllowBorderRounding)
                {
                    rect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
                    rect.Inflate(0, .5);
                }
                 if (this.UseGuidelineSetToRenderBackground)
               {
                    var guidelines = new GuidelineSet();
                    guidelines.GuidelinesX.Add(rect.Left);
                    guidelines.GuidelinesX.Add(rect.Right);
                    guidelines.GuidelinesY.Add(rect.Top);
                    guidelines.GuidelinesY.Add(rect.Bottom);
                    dc.PushGuidelineSet(guidelines);
                }
                dc.DrawRectangle(background, null, rect);
            }
#if MEASURETIME
            }
#endif
        }

        private void NeedsToDrawBackground(IRenderCellInfo cellInfo, int rowIndex, int colIndex, out Brush background, out bool backgroundDrawn)
        {
            background = GetCellBackground(cellInfo, false);
            backgroundDrawn = background == null || combinedCellBackgroundIdsTable.Count > 0 && combinedCellBackgroundIdsTable[rowIndex][colIndex] == combinedCellBackgroundArrangeId && background == GetCellBackground(cellInfo, true);
        }

        private void NeedsToDrawBackground(RenderCellArgs rca, out Brush background, out bool backgroundDrawn)
        {
            this.NeedsToDrawBackground(rca.CellInfo, rca.VisibleRow.VisibleIndex, rca.VisibleColumn.VisibleIndex, out  background, out backgroundDrawn);
        }

        #endregion
        #region InvalidateCellBackground
        /// <summary>
        /// Invalidates the cell background. The cells control combines the background
        /// of neighbouring cells in the <see cref="OnArrangeContent"/> method and reuses 
        /// this information whenever the cells control is control is rendered without 
        /// rearranging contents (when you specify false as paramater to the 
        /// <see cref="ScrollControl.InvalidateVisual(System.Boolean)"/>
        /// method). Call this method to ensure that background for this individual cell
        /// is requeried next time the cells control is rendered.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        public void InvalidateCellBackground(int rowIndex, int columnIndex)
        {
            InvalidateCellBackground(rowIndex, columnIndex, false);
            //if (combinedCellBackgroundIdsTable.Count > 0)
            //{
            //    VisibleLineInfo visibleRow = ScrollRows.GetVisibleLineAtLineIndex(rowIndex);
            //    VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(columnIndex);
            //    if (visibleRow != null && visibleColumn != null)
            //    {
            //        backgroundsDirty = true;
            //    }
            //}
        }

        public void InvalidateCellBackground(int rowIndex, int columnIndex, bool useIndividualCells)
        {
            if (combinedCellBackgroundIdsTable.Count > 0)
            {
                VisibleLineInfo visibleRow = ScrollRows.GetVisibleLineAtLineIndex(rowIndex);
                VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(columnIndex);
                if (visibleRow != null && visibleColumn != null)
                {
                    if (useIndividualCells)
                    {
                        IndividualCellBackgroundsToDraw.Add(new RowColumnIndex(rowIndex, columnIndex));
                    }
                    else
                    {
                        IndividualCellBackgroundsToDraw.Clear();
                        backgroundsDirty = true;
                    }
                }
            }
        }
         
        private HashSet<RowColumnIndex> individualCellBackgroundsToDraw = null;

        private class RowColumnIndexComparer : IEqualityComparer<RowColumnIndex>
        {
            #region IEqualityComparer<RowColumnIndex> Members

            public bool Equals(RowColumnIndex x, RowColumnIndex y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(RowColumnIndex obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }

        protected HashSet<RowColumnIndex> IndividualCellBackgroundsToDraw
        {
            get
            {
                if (individualCellBackgroundsToDraw == null)
                    individualCellBackgroundsToDraw = new HashSet<RowColumnIndex>(new RowColumnIndexComparer());
                return individualCellBackgroundsToDraw;
            }
            set { individualCellBackgroundsToDraw = value; }
        }
        

        /// <summary>
        /// Invalidates the cell border. This will make sure the cells control
        /// will rerender cell borders next time OnRender is called.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        public void InvalidateCellBorder(RowColumnIndex cellRowColumnIndex)
        {
            InvalidateCellBorder(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        /// <summary>
        /// Invalidates the cell border. This will make sure the cells control
        /// will rerender cell borders next time OnRender is called.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        public void InvalidateCellBorder(int rowIndex, int columnIndex)
        {
            VisibleLineInfo visibleRow = ScrollRows.GetVisibleLineAtLineIndex(rowIndex);
            VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(columnIndex);
            if (visibleRow != null && visibleColumn != null)
            {
                this.bordersDirty = true;
            }
        }


        /// <summary>
        /// Invalidates the cell background. The cells control combines the background
        /// of neighbouring cells in the <see cref="OnArrangeContent"/> method and reuses
        /// this information whenever the cells control is control is rendered without
        /// rearranging contents (when you specify false as paramater to the 
        /// <see cref="ScrollControl.InvalidateVisual(System.Boolean)"/>
        /// method). Call this method to ensure that background for this individual cell
        /// is requeried next time the cells control is rendered.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        public void InvalidateCellBackground(RowColumnIndex cellRowColumnIndex)
        {
            InvalidateCellBackground(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        /// <summary>
        /// The method currently does not do anything. It is a place holder to be able
        /// to invalidate the blinking of a cell without touching the cell content, but
        /// with the current implementation this is actually not needed. For now we left
        /// this method in case we change the implementation how blinking cells are
        /// drawn.
        /// </summary>
        /// <param name="cellRowColumnIndex"></param>
        public void InvalidateCellRenderStyleBackground(RowColumnIndex cellRowColumnIndex)
        {
        }

        #endregion
        #region RenderCoveredCells
        /// <summary>
        /// Renders the covered cells.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        protected virtual void RenderCoveredCells(DrawingContext dc)
        {
            if (coveredCellsLayout == null)
                return;

#if MEASURETIME
            //using (MeasureTime.Measure("VirtualizingCellsControl.RenderCoveredCells"))
            {
#endif

            Point corner = new Point(ScrollColumns.ViewCorner, ScrollRows.ViewCorner);
            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            double viewWidth = ScrollColumns.ViewSize;
            visibleCoveredCellsArrangeId++;


            foreach (VisibleCoveredCellInfo ccSpan in coveredCellsLayout)
            {
                if (ccSpan.IsAmbiguousSection)
                    continue;

                //if (ccSpan.IsAmbiguousSection)
                //{
                //    IDrawCellInfo ci = InternalGetCellInfo(ccSpan.CoveredCell);
                //    if (GetCellUIElements(ccSpan.CoveredCell.Top, ccSpan.CoveredCell.Left) != null)
                //        continue;
                //}

                VisibleLineInfo visibleRow = ScrollRows.GetVisibleLines()[ccSpan.Top];
                VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLines()[ccSpan.Left];
                Rect cellRect = ccSpan.ExactBounds;
                IRenderCellInfo renderCellInfo = InternalGetCellInfo(visibleRow, visibleColumn);
                RenderCellArgs rca = new RenderCellArgs(this, visibleRow, visibleColumn, cellRect, renderCellInfo);
                rca.VisibleCoveredCellInfo = ccSpan;

                if (isRenderCellBackgroundPhase)
                {
                    RenderCoveredCell(dc, ccSpan, rca);
                }
                else
                {
                    if (useDrawingVisualForCells)
                    {
                        VirtualizingCellsControlChildFrame canvas = (VirtualizingCellsControlChildFrame)GetChildFrame(rca, this.InnerFrame);

                        DrawingVisual dv;
                        if (canvas.RenderedCells.TransformOrCreateDrawingVisual(rca, out dv))
                        {
                            using (DrawingContext dc2 = dv.RenderOpen())
                            {
                                RenderCoveredCell(dc2, ccSpan, rca);
                            }
                        }
                    }
                    else
                    {
                        RenderCoveredCell(dc, ccSpan, rca);
                    }
                }
            }
#if MEASURETIME
            }
#endif
        }

        private void RenderCoveredCell(DrawingContext dc2, VisibleCoveredCellInfo ccSpan, RenderCellArgs rca)
        {
            bool clip = (isRenderCellBackgroundPhase || !useDrawingVisualForCells) && ccSpan.ClippedBounds != ccSpan.ExactBounds;
            if (clip) PushClip(dc2, ccSpan.ClippedBounds);
            if (isRenderCellBackgroundPhase)
                OnRenderCellBackground(dc2, ccSpan, rca);
            else
                OnRenderCell(dc2, rca);
            if (clip) dc2.Pop();
        }

        protected virtual void RenderOverlappingCells(DrawingContext dc)
        {
            if (overlappingCellsLayout == null)
                return;

#if MEASURETIME
            //using (MeasureTime.Measure("VirtualizingCellsControl.RenderCoveredCells"))
            {
#endif

            Point corner = new Point(ScrollColumns.ViewCorner, ScrollRows.ViewCorner);
            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            double viewWidth = ScrollColumns.ViewSize;
            visibleOverlappingCellsArrangeId++;


            foreach (VisibleOverlappingCellInfo icSpan in overlappingCellsLayout)
            {
                if (icSpan.IsAmbiguousSection)
                    continue;
                
                VisibleLineInfo visibleRow = ScrollRows.GetVisibleLines()[icSpan.Top];
                VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLines()[icSpan.Left];
                var visbibleImageCells = overlappingCellsLayout.GetVisibleCellSpan(visibleRow, visibleColumn);
                Rect cellRect = icSpan.ExactBounds;
                IRenderCellInfo renderCellInfo = InternalGetCellInfo(visibleRow, visibleColumn);
                RenderCellArgs rca = new RenderCellArgs(this, visibleRow, visibleColumn, cellRect, renderCellInfo);
                rca.VisibleoverlappingCellInfo = icSpan;

                if (isRenderCellBackgroundPhase)
                {
                    RenderOverlappingCell(dc, icSpan, rca);
                }
                else
                {
                    if (useDrawingVisualForCells)
                    {
                        VirtualizingCellsControlChildFrame canvas = (VirtualizingCellsControlChildFrame)GetChildFrame(rca, this.InnerFrame);

                        DrawingVisual dv;
                        if (canvas.RenderedCells.TransformOrCreateDrawingVisual(rca, out dv))
                        {
                            using (DrawingContext dc2 = dv.RenderOpen())
                            {
                                RenderOverlappingCell(dc2, icSpan, rca);
                            }
                        }
                    }
                    else
                    {
                        RenderOverlappingCell(dc, icSpan, rca);
                    }
                }
            }
#if MEASURETIME
            }
#endif
        }

        private void RenderOverlappingCell(DrawingContext dc2, VisibleOverlappingCellInfo icSpan, RenderCellArgs rca)
        {
            bool clip = (isRenderCellBackgroundPhase || !useDrawingVisualForCells) && icSpan.ClippedBounds != icSpan.ExactBounds;
            if (clip) PushClip(dc2, icSpan.ClippedBounds);
            if (isRenderCellBackgroundPhase)
                OnRenderCellBackground(dc2, icSpan, rca);
            else
                OnRenderCell(dc2, rca);
            if (clip) dc2.Pop();
        }

        #endregion
        #region RenderCellBorders
        /// <summary>
        /// Renders the cell borders.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        protected virtual void RenderCellBorders(DrawingContext dc)
        {            

#if MEASURETIME
            //using (MeasureTime.Measure("VirtualizingCellsControl.RenderCellBorders"))
            {
#endif
            if (bordersDirty)
            {
                // InvalidateCellBackground was called and there has been no
                // OnArrangeContent call since then (probably because Invalidate(false) was called
                // which will skip OnArrangeContent in layout pass).

                // Calculate Cells Background - combine cells with same brush into one draw operation.
                ArrangeCellBorders(RenderSize);
            }

            ClearBorderFrames();

            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();

            if (CanDrawHorizontalLineFirst())
            {
                RenderHorizaontalLines(dc, visibleRows, visibleColumns);
                RenderVerticalLines(dc, visibleRows, visibleColumns);
            }
            else
            {
                RenderVerticalLines(dc, visibleRows, visibleColumns);
                RenderHorizaontalLines(dc, visibleRows, visibleColumns);
            }
           

#if MEASURETIME
            }
#endif

            CloseBorderFrameDrawingContext();
        }

        protected virtual bool CanDrawHorizontalLineFirst()
        {
            return false;
        }


        private DrawingContext GetBorderFrameDrawingContext(VirtualizingCellsControlChildFrame canvas)
        {
            if (canvas.DrawingContext != null)
            {
                return canvas.DrawingContext;
            }

            DrawingVisual dv;
            if (canvas.Children.Count == 0)
            {
                dv = new NoHitTestDrawingVisual();
                canvas.Children.Add(dv);
            }
            else
                dv = (DrawingVisual)canvas.Children[0];

            canvas.DrawingContext = dv.RenderOpen();

            return canvas.DrawingContext;
        }

        private void CloseBorderFrameDrawingContext()
        {
            foreach (VirtualizingCellsControlChildFrame frame in backgroundFrameBorders.Children)
            {
                if (frame.DrawingContext != null)
                {
                    frame.DrawingContext.Close();
                    frame.DrawingContext = null;
                }
            }

        }

        private void ClearBorderFrames()
        {
            foreach (VirtualizingCellsControlChildFrame frame in backgroundFrameBorders.Children)
            {
                while (frame.Children.Count > 0)
                    frame.Children.RemoveAt(0);
            }

        }

        internal void RenderHorizaontalLines(DrawingContext dc, VisibleLinesCollection visibleRows, VisibleLinesCollection visibleColumns)
        {
            bool hasOriginMargin = Margin.Left > 0;
            bool hasCornerMargin = Margin.Right > 0; 
            
            foreach (CellBorderRangeList innerBorders in horizontalLines)
            {
                VisibleLineInfo visibleRow = innerBorders.visibleLine;
                CellBorderSide borderSide = innerBorders.borderSide;

                if (borderSide == CellBorderSide.Top && visibleRow.IsClippedOrigin
                    || borderSide == CellBorderSide.Bottom && visibleRow.IsClippedCorner)
                    continue;

                foreach (CellBorderRange borderRange in innerBorders)
                {
                    VisibleLineInfo firstColumn = visibleColumns[borderRange.first];
                    VisibleLineInfo lastColumn = visibleColumns[borderRange.last];

                    Rect r;
                    bool needsClip = (firstColumn.IsClippedBodyOrigin(hasOriginMargin) || lastColumn.IsClippedBodyCorner(hasCornerMargin)) 
                        && borderRange.border.DashStyle != DashStyles.Solid;

                    if (needsClip)
                    {
                        r = GridUtil.FromLTRB(firstColumn.Origin, visibleRow.Origin, lastColumn.Corner, visibleRow.Corner);

                        bool isAtLeftSide = firstColumn.IsHeader;
                        bool isAtRightSide = lastColumn.IsFooter;

                        VirtualizingCellsControlChildFrame canvas = (VirtualizingCellsControlChildFrame)GetChildFrame(isAtLeftSide, true, isAtRightSide, true, backgroundFrameBorders);
                        DrawingContext dc2 = GetBorderFrameDrawingContext(canvas);
                        OnRenderBorder(dc2, r, r, borderSide, borderRange.border);
                    }
                    else
                    {
                        r = GridUtil.FromLTRB(firstColumn.ClippedOrigin, visibleRow.Origin, lastColumn.ClippedCorner, visibleRow.Corner);
                        OnRenderBorder(dc, r, r, borderSide, borderRange.border);
                    }
                }
            }
        }
        
#if ENABLE_PARTIAL_TRUST
        [SecurityCritical]
#endif
        internal void RenderVerticalLines(DrawingContext dc, VisibleLinesCollection visibleRows, VisibleLinesCollection visibleColumns)
        {
            bool hasOriginMargin = Margin.Top > 0;
            bool hasCornerMargin = Margin.Bottom > 0; 
            
            foreach (CellBorderRangeList innerBorders in verticalLines)
            {
                VisibleLineInfo visibleColumn = innerBorders.visibleLine;
                CellBorderSide borderSide = innerBorders.borderSide;

                if (borderSide == CellBorderSide.Left && visibleColumn.IsClippedOrigin
                    || borderSide == CellBorderSide.Right && visibleColumn.IsClippedCorner)
                    continue;

                foreach (CellBorderRange borderRange in innerBorders)
                {
                    VisibleLineInfo firstRow = visibleRows[borderRange.first];
                    VisibleLineInfo lastRow = visibleRows[borderRange.last];

                    Rect r;
                    bool needsClip = (firstRow.IsClippedBodyOrigin(hasOriginMargin) || lastRow.IsClippedBodyCorner(hasCornerMargin)) && borderRange.border.DashStyle != DashStyles.Solid;
                    if (needsClip)
                    {
                        r = GridUtil.FromLTRB(visibleColumn.Origin, firstRow.Origin, visibleColumn.Corner, lastRow.Corner);

                        bool isAtTop = firstRow.IsHeader;
                        bool isAtBottom = lastRow.IsFooter;

                        VirtualizingCellsControlChildFrame canvas = (VirtualizingCellsControlChildFrame)GetChildFrame(true, isAtTop, true, isAtBottom, backgroundFrameBorders);
                        DrawingContext dc2 = GetBorderFrameDrawingContext(canvas);
                        OnRenderBorder(dc2, r, r, borderSide, borderRange.border);
                    }
                    else
                    {
                        r = GridUtil.FromLTRB(visibleColumn.Origin, firstRow.ClippedOrigin, visibleColumn.Corner, lastRow.ClippedCorner);
                        OnRenderBorder(dc, r, r, borderSide, borderRange.border);
                    }
                  
                }
            }
        }

        /// <summary>
        /// A helper method that createa a <see cref="RectangleGeometry"/>, calls Freeze and
        /// <see cref="DrawingContext.PushClip"/>
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="clipRect">The clip rect.</param>
        protected void PushClip(DrawingContext dc, Rect clipRect)
        {
            RectangleGeometry rg = new RectangleGeometry(clipRect);
            rg.Freeze();
            dc.PushClip(rg);
        }

        /// <summary>
        /// A helper method that creates a <see cref="CombinedGeometry"/> consisting of a
        /// <see cref="RectangleGeometry"/> which excludes the given excludeRect, calls Freeze and
        /// <see cref="DrawingContext.PushClip"/>
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="clipRect">The clip rect.</param>
        /// <param name="excludeRect">The exclude rect.</param>
        protected void PushClip(DrawingContext dc, Rect clipRect, Rect excludeRect)
        {
            RectangleGeometry rg = new RectangleGeometry(clipRect);
            rg.Freeze();
            if (excludeRect.IsEmpty)
            {
                dc.PushClip(rg);
            }
            else
            {
                RectangleGeometry rg2 = new RectangleGeometry(excludeRect);
                rg2.Freeze();
                CombinedGeometry crg = new CombinedGeometry(GeometryCombineMode.Exclude, rg, rg2);
                crg.Freeze();
                dc.PushClip(crg);
            }
        }

        bool useGuidelineSetToRenderBorder = false;

        /// <summary>
        /// Use guide lines to adjusting rendered border to a device pixel grid.
        /// </summary>
        public bool UseGuidelineSetToRenderBorder
        {
            get { return useGuidelineSetToRenderBorder; }
            set { useGuidelineSetToRenderBorder = value; }
        }

        bool useGuidelineSetToRenderBackground = false;
        public bool UseGuidelineSetToRenderBackground
        {
            get { return useGuidelineSetToRenderBackground; }
            set { useGuidelineSetToRenderBackground = value; }
        }
#if ENABLE_PARTIAL_TRUST
        [SecurityCritical]
#endif
        protected virtual void OnRenderBorder(DrawingContext dc, Rect cellRect, Rect clipRect, CellBorderSide borderSide, Pen pen)
        {
            if (cellRect.Width == 0 || pen == null || pen.Brush == null)
                return;

            if (pen.Thickness == 0 && ShowGridLines)
            {
                pen.Brush = new SolidColorBrush(Color.FromArgb(255, 218, 220, 221));
                pen.Thickness = 0.5;
            }
            //cellRect = clipRect;
            cellRect = new Rect(Math.Round(cellRect.Left), Math.Round(cellRect.Top), Math.Round(cellRect.Width), Math.Round(cellRect.Height));
            if (UseGuidelineSetToRenderBorder)
            {
                Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                double dpiFactor = 1 / m.M11;
                double halfPenWidth = pen.Thickness / 2 * dpiFactor;
                // Create a guidelines set
                GuidelineSet guidelines = new GuidelineSet();
                guidelines.GuidelinesX.Add(cellRect.Left + halfPenWidth);
                guidelines.GuidelinesX.Add(cellRect.Right + halfPenWidth);
                guidelines.GuidelinesY.Add(cellRect.Top + halfPenWidth);
                guidelines.GuidelinesY.Add(cellRect.Bottom + halfPenWidth);
                dc.PushGuidelineSet(guidelines);
            }
            switch (borderSide)
            {
                case CellBorderSide.Top:
                    dc.DrawLine(pen, cellRect.TopLeft, cellRect.TopRight);
                    //Console.WriteLine("dc.DrawLine({0}, {1});", cellRect.TopLeft, cellRect.TopRight);
                    break;
                case CellBorderSide.Bottom:
                    dc.DrawLine(pen, cellRect.BottomLeft, cellRect.BottomRight);
                    //Console.WriteLine("dc.DrawLine({0}, {1});", cellRect.BottomLeft, cellRect.BottomRight);
                    break;
                case CellBorderSide.Left:
                    dc.DrawLine(pen, cellRect.TopLeft, cellRect.BottomLeft);
                    //Console.WriteLine("dc.DrawLine({0}, {1});", cellRect.TopLeft, cellRect.BottomLeft);
                    break;
                case CellBorderSide.Right:
                    dc.DrawLine(pen, cellRect.TopRight, cellRect.BottomRight);
                    //Console.WriteLine("dc.DrawLine({0}, {1});", cellRect.TopRight, cellRect.BottomRight);
                    break;
            }
            if (UseGuidelineSetToRenderBorder)
                dc.Pop();
        }
        #endregion

        #endregion

        #region Measure
        /// <summary>
        /// Returns the maximum height and width of the cells control. If pixel scrolling
        /// is enabled for a axis the method queries the <see cref="PixelScrollAxis.TotalExtent"/>
        /// value of the <see cref="PixelScrollAxis"/>.
        /// </summary>
        /// <param name="constraint">The maximum size limit for the control.</param>
        /// <returns>The maximum size of the control.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (ScrollOwner != null && ScrollOwner.CanContentScroll)
            {
                // Grid does all scrolling.
                // TODO: Support MeasureOverride for size to fit
                if (!IsDoubleValueSet(FrameworkElement.HeightProperty))
                {
                    if (ScrollRows is PixelScrollAxis)
                        constraint.Height = Math.Min(constraint.Height, ((PixelScrollAxis)ScrollRows).TotalExtent + 1);
                }

                if (!IsDoubleValueSet(FrameworkElement.WidthProperty))
                {
                    var isInfinite = (constraint.Width == double.PositiveInfinity || constraint.Width == double.NegativeInfinity);
                    if ((ScrollColumns is PixelScrollAxis) && (!CanAutoCalculateWidth() || isInfinite))
                            constraint.Width = Math.Min(constraint.Width, ((PixelScrollAxis)ScrollColumns).TotalExtent + 1);
                }
            }
            else
            {
                if (!IsDoubleValueSet(FrameworkElement.HeightProperty))
                {
                    // Grid is shown in ViewBox or another panel - does no scrolling by itsself.
                    if (ScrollRows is PixelScrollAxis)
                        constraint.Height = ((PixelScrollAxis)ScrollRows).TotalExtent + 1;
                }

                if (!IsDoubleValueSet(FrameworkElement.WidthProperty))
                {
                    var isInfinite = (constraint.Width == double.PositiveInfinity || constraint.Width == double.NegativeInfinity);
                    if ((ScrollColumns is PixelScrollAxis) && (!CanAutoCalculateWidth() || isInfinite))
                            constraint.Width = ((PixelScrollAxis)ScrollColumns).TotalExtent + 1;
                }

                // TODO: MeasureOverride - MaxWidth, MinWidth
            }

            return base.MeasureOverride(constraint);
        }

        bool IsDoubleValueSet(DependencyProperty dp)
        {
            object value = GetValue(dp);
            return value != DependencyProperty.UnsetValue && !double.IsNaN((double)value);
        }
        #endregion

        #region Arrange

        /// <summary>
        /// Called when settings of <see cref="ScrollAxisControl.ScrollRows"/> or 
        /// <see cref="ScrollAxisControl.ScrollColumns"/> were changed.
        /// </summary>
        protected override void OnScrollLayoutChanged()
        {
            if (coveredCellsLayout != null)
                coveredCellsLayout.SetDirty();

            if (overlappingCellsLayout != null)
                overlappingCellsLayout.SetDirty();

            base.OnScrollLayoutChanged();
        }
        /// <summary>
        /// Called when the <see cref="ScrollControl.InvalidateVisual(System.Boolean)"/> method was called.
        /// </summary>
        /// <param name="isArrangeDirty">if set to <c>true</c> indicates that <see cref="ScrollControl.OnArrangeContent"/>
        /// will be called when control gets updated. Otherwise the OnArrangeContent will be skipped
        /// and only OnRender will be called.</param>
        protected override void OnInvalidated(bool isArrangeDirty)
        {
            if (isArrangeDirty)
                SetCellLayoutDirty();

            if (overlappingCellsLayout != null)
                overlappingCellsLayout.SetDirty();

            base.OnInvalidated(isArrangeDirty);
        }

        protected void SetCellLayoutDirty()
        {
            backgroundsDirty = true;
            bordersDirty = true;
            if (coveredCellsLayout != null)
                coveredCellsLayout.SetDirty();
            if (overlappingCellsLayout != null)
                overlappingCellsLayout.SetDirty();
        }


        #region OnArrangeContent

        protected override Size OnArrangeOverride(Size arrangeSize, ref bool isArrangeDirty)
        {
            Size size = base.OnArrangeOverride(arrangeSize, ref isArrangeDirty);
            ArrangeFrames(arrangeSize, backgroundFrameBorders.Children);
            return size;
        }

        /// <summary>
        /// Arranges the cells row by row with the <see cref="ArrangeCellUIElements"/> method. For each cell
        /// the virtual <see cref="OnArrangeCell"/> method is called. OnArrangeCell gets
        /// the <see cref="ICellRenderer"/> for a cell and calls its <see cref="ICellRenderer.Arrange"/>
        /// method.<para/>
        /// ArrangeCellUIElements creates new UIElements for cells scrolled into view or unload UIElements for 
        /// cells scrolled out of view. <para/>
        /// OnArrangeContent also arranges covered cells, spaned backgrounds, 
        /// cell borders and combines cells with same cell backround 
        /// (<see cref="ArrangeCombinedCellBackgrounds"/>) to reduce number 
        /// of drawing operations.
        /// </summary>
        /// <param name="arrangeSize"></param>
        protected override void OnArrangeContent(Size arrangeSize)
        {
            //Trace.WriteLine("OnArrangeContent");
            this.arrangeSize = arrangeSize;
            // Covered cells
            ArrangeCoveredCells(arrangeSize);

            //Overlapping Cells
            ArrangeOverlappingCells(arrangeSize);

            // Draw one background across multiple cells
            ArrangeCellSpanBackgrounds(arrangeSize);

            // Create new UIElements for cells scrolled into view or unload UIElements for cells scrolled out of view.
            ArrangeCellUIElements(arrangeSize);

            // Calculate Borders - they will be rendered later in OnRender.
            ArrangeCellBorders(arrangeSize);

            // Calculate Cells Background - combine cells with same brush into one draw operation.
            ArrangeCombinedCellBackgrounds(arrangeSize);
#if SILVERLIGHT

            RenderCellBackgounds();

            RenderCellBorders();
#endif
            hasReArranged = false;
        }
        #endregion

        #region CombineCellBackgrounds

        /// <summary>
        /// Allocates an grid of integer values and ensures enough entries
        /// are there for each visible row and column.
        /// </summary>
        void PrepareCombinedCellBackgroundIdsTable()
        {
            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();
            int c = visibleColumns.Count;
            int r = visibleRows.Count;

            // if (combinedCellBackgroundIdsTable.Count > 0)
            {
                for (int n = 0; n < combinedCellBackgroundIdsTable.Count; n++)
                    PrepareCombinedCellBackgroundIdsTableRow(combinedCellBackgroundIdsTable[n], c);
            }
            while (combinedCellBackgroundIdsTable.Count < r)
            {
                List<int> arrangeIds = new List<int>();
                PrepareCombinedCellBackgroundIdsTableRow(arrangeIds, c);
                combinedCellBackgroundIdsTable.Add(arrangeIds);
            }

            combinedCellBackgroundArrangeId++;
        }

        void PrepareCombinedCellBackgroundIdsTableRow(List<int> arrangeIds, int count)
        {
            while (arrangeIds.Count < count)
            {
                arrangeIds.Add(0);
            }
        }

        /// <summary>
        /// Combines cells with same cell backround to reduce number 
        /// of drawing operations. The cell background will later be rendered
        /// when <see cref="OnRender"/> is called.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        protected virtual void ArrangeCombinedCellBackgrounds(Size arrangeSize)
        {
            backgroundsDirty = false;
            needRenderBackgrounds = true;
            PrepareCombinedCellBackgroundIdsTable();
            combinedCellBackgroundsList.Clear();

            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();

            // section: 0 - Header, 1 - Body, 2 - Footer
            for (int rowSection = 0; rowSection < 3; rowSection++)
            {
                int firstRow, lastRow;
                ScrollRows.GetVisibleSection(rowSection, out firstRow, out lastRow);

                for (int visibleRowIndex = firstRow; visibleRowIndex <= lastRow; visibleRowIndex++)
                {
                    VisibleLineInfo visibleRow = visibleRows[visibleRowIndex];

                    // section: 0 - Header, 1 - Body, 2 - Footer
                    for (int section = 0; section < 3; section++)
                    {
                        VisibleCombinedCellBackgroundInfo backgroundSpan = null;
                        IRenderCellInfo previousCellInfo = null;
                        int firstColumn, lastColumn;
                        ScrollColumns.GetVisibleSection(section, out firstColumn, out lastColumn);

                        for (int visibleColumnIndex = firstColumn; visibleColumnIndex <= lastColumn; visibleColumnIndex++)
                        {
                            bool skip = combinedCellBackgroundIdsTable[visibleRowIndex][visibleColumnIndex] == combinedCellBackgroundArrangeId;
                            // Skip this if cell belongs to an CellBackgroundSpan from a previous row.
                            if (backgroundSpan == null && skip)
                                continue;

                            VisibleLineInfo visibleColumn = visibleColumns[visibleColumnIndex];
                            IRenderCellInfo ci = InternalGetCellInfo(visibleRow, visibleColumn);
                            if (backgroundSpan != null)
                            {
                                // CellInfo will be same for cells inside covered cell.
                                if (!skip
                                    && (Object.ReferenceEquals(previousCellInfo, ci) || previousCellInfo.CanCombineCellBackground(ci)))
                                {
                                    backgroundSpan.right = visibleColumnIndex;
                                    continue;
                                }

                                // At this point CellBackgroundSpan contains information about 
                                // columns in same row with same background. Now check neighboring
                                // rows that have same background for exact the same colors.

                                CheckSubsequentRows(visibleRowIndex, lastRow, backgroundSpan, previousCellInfo);
                            }

                            Brush background = GetCellBackground(ci, true);
                            if (background != null)
                                backgroundSpan = new VisibleCombinedCellBackgroundInfo(visibleRowIndex, visibleColumnIndex, background);
                            else
                                backgroundSpan = null;

                            previousCellInfo = ci;
                        }
                        if (backgroundSpan != null)
                        {
                            backgroundSpan.right = lastColumn;
                            CheckSubsequentRows(visibleRowIndex, lastRow, backgroundSpan, previousCellInfo);
                        }
                    }
                }
            }
        }

        private void CheckSubsequentRows(int visibleRowIndex, int lastRow, VisibleCombinedCellBackgroundInfo combinedBackground, IRenderCellInfo previousCellInfo)
        {
            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();

            bool match = true;
            for (int nextRowIndex = visibleRowIndex + 1; nextRowIndex <= lastRow; nextRowIndex++)
            {
                VisibleLineInfo nextRow = visibleRows[nextRowIndex];
                for (int nextColumnIndex = combinedBackground.left; nextColumnIndex <= combinedBackground.right; nextColumnIndex++)
                {
                    VisibleLineInfo nextColumn = visibleColumns[nextColumnIndex];
                    bool skip = combinedCellBackgroundIdsTable[visibleRowIndex][nextColumnIndex] == combinedCellBackgroundArrangeId;
                    if (skip)
                    {
                        match = false;
                        break;
                    }
                    IRenderCellInfo nextci = InternalGetCellInfo(nextRow, nextColumn);
                    // CellInfo will be same for cells inside covered cell.
                    if (!Object.ReferenceEquals(previousCellInfo, nextci) && !previousCellInfo.CanCombineCellBackground(nextci))
                    {
                        match = false;
                        break;
                    }
                }
                if (!match)
                {
                    combinedBackground.bottom = nextRowIndex - 1;
                    break;
                }
            }
            if (match)
                combinedBackground.bottom = lastRow;

            if (combinedBackground.Right > combinedBackground.Left || combinedBackground.Bottom > combinedBackground.Top)
            {
                for (int nextRowIndex = combinedBackground.top; nextRowIndex <= combinedBackground.bottom; nextRowIndex++)
                {
                    for (int nextColumnIndex = combinedBackground.left; nextColumnIndex <= combinedBackground.right; nextColumnIndex++)
                    {
                        combinedCellBackgroundIdsTable[nextRowIndex][nextColumnIndex] = combinedCellBackgroundArrangeId;
                    }
                }
                combinedCellBackgroundsList.Add(combinedBackground);
            }
        }

        /// <summary>
        /// Gets the cell background for a cell from the <see cref="IRenderCellInfo"/> cell style.
        /// </summary>
        /// <param name="ci">The cell style.</param>
        /// <param name="combineBackgrounds">if set to <c>true</c> indicates that the method was
        /// called during OnArrangeContent when control is combining background of neighbouring cells
        /// to be drawn in single batches.</param>
        /// <returns></returns>
        protected virtual Brush GetCellBackground(IRenderCellInfo ci, bool combineBackgrounds)
        {
            // If background stored in CellInfo is not a brush and instead a description of a brush (e.g. GridBrushInfo)
            // this method is a good place to convert that to a brush and cache it for reuse in other cells.
            return ci.GetCellBackground() as Brush;
        }
        #endregion

        #region CoveredCells

        /// <summary>
        /// Gets a covered cell from the <see cref="CoveredCellsProvider"/> that includes
        /// the specified cells row and column index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        public CoveredCellInfo GetCoveredCell(int rowIndex, int columnIndex)
        {
            if (coveredCellsProvider == null)
                return null;

            return coveredCellsProvider.GetCoveredCell(rowIndex, columnIndex);
        }

        /// <summary>
        /// Gets a covered cell from the <see cref="CoveredCellsProvider"/> that includes
        /// the specified cells row and column index.
        /// </summary>
        /// <param name="cellRowColumnIndex">The cells row and column index.</param>
        /// <returns></returns>
        public CoveredCellInfo GetCoveredCell(RowColumnIndex cellRowColumnIndex)
        {
            return GetCoveredCell(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        /// <summary>
        /// Arranges the covered cells.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        protected virtual void ArrangeCoveredCells(Size arrangeSize)
        {
            if (coveredCellsProvider == null)
                return;

            if (coveredCellsLayout == null)
                coveredCellsLayout = new VisibleCellSpanLayout<VisibleCoveredCellInfo>(this, new GetCellSpanDelegate(GetCoveredCell));

            coveredCellsLayout.ArrangeCellSpans(arrangeSize, coveredCellsProvider.IsEmpty);
        }

        private Size arrangeSize;
        bool hasReArranged = false;
        /// <summary>
        /// Force and Rearrange the CoveredCells in the View
        /// </summary>
        /// <remarks>
        /// While adding coveredcells in QueryCellInfo , need to call this method to refesh the Covered Cells Layout
        /// </remarks>
        public static void ReArrangeCoveredCells(VirtualizingCellsControl virtualizingCellsControl)
        {
            if (virtualizingCellsControl != null && !virtualizingCellsControl.arrangeSize.IsEmpty)
            {
                virtualizingCellsControl.ArrangeCoveredCells(virtualizingCellsControl.arrangeSize);
                virtualizingCellsControl.hasReArranged = true;
            }
        }

        protected VisibleCoveredCellInfo GetVisibleCoveredCell(VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn)
        {
            if (coveredCellsProvider != null && coveredCellsLayout != null && !coveredCellsProvider.IsEmpty)
                return coveredCellsLayout.GetVisibleCellSpan(visibleRow, visibleColumn);
            return null;
        }

        CoveredCellInfo GetCoveredCell(VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn)
        {
            VisibleCoveredCellInfo ccSpan = GetVisibleCoveredCell(visibleRow, visibleColumn);
            if (ccSpan != null)
                return ccSpan.CoveredCell;

            return null;
        }

        //Overlapping Cell
        public OverlappingCellInfo GetOverlappingCell(int rowIndex, int columnIndex)
        {
            if (overlappingCellsProvider == null)
                return null;

            return overlappingCellsProvider.GetOverlappingCell(rowIndex, columnIndex);
        }

        public OverlappingCellInfo GetOverlappingCell(RowColumnIndex cellRowColumnIndex)
        {
            return GetOverlappingCell(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        protected virtual void ArrangeOverlappingCells(Size arrangeSize)
        {            
            if (overlappingCellsProvider == null || overlappingCellsProvider.IsEmpty)
                return;

            if (overlappingCellsLayout == null)
                overlappingCellsLayout = new VisibleCellSpanLayout<VisibleOverlappingCellInfo>(this, new GetCellSpanDelegate(GetOverlappingCell));

            overlappingCellsLayout.ArrangeCellSpans(arrangeSize, overlappingCellsProvider.IsEmpty);
        }

        protected VisibleOverlappingCellInfo GetVisibleOverlappingCell(VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn)
        {
            if (overlappingCellsProvider != null && overlappingCellsLayout != null && !overlappingCellsProvider.IsEmpty)
                return overlappingCellsLayout.GetVisibleCellSpan(visibleRow, visibleColumn);
            return null;
        }

        OverlappingCellInfo GetOverlappingCell(VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn)
        {
            VisibleOverlappingCellInfo icSpan = GetVisibleOverlappingCell(visibleRow, visibleColumn);
            if (icSpan != null)
                return icSpan.OverlappingCell;
            return null;
        }


        #endregion

        #region CellSpanBackgrounds

        /// <summary>
        /// Gets the cell span backgrounds from the <see cref="CellSpanBackgroundsProvider"/> that include
        /// the specified cells row and column index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        public List<CellSpanBackgroundInfo> GetCellSpanBackgrounds(int rowIndex, int columnIndex)
        {
            if (cellSpanBackgroundsProvider == null)
                return null;
            return cellSpanBackgroundsProvider.GetCellSpanBackgrounds(rowIndex, columnIndex);
        }

        /// <summary>
        /// Gets the cell span backgrounds from the <see cref="CellSpanBackgroundsProvider"/> that includes
        /// the specified cells row and column index.
        /// </summary>
        /// <param name="cellRowColumnIndex">The pos.</param>
        /// <returns></returns>
        public List<CellSpanBackgroundInfo> GetCellSpanBackgrounds(RowColumnIndex cellRowColumnIndex)
        {
            return GetCellSpanBackgrounds(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        /// <summary>
        /// Arranges the cell span backgrounds.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        protected virtual void ArrangeCellSpanBackgrounds(Size arrangeSize)
        {
            if (cellSpanBackgroundsProvider == null)
                return;

            framesListWithBackgoundSpans = new List<List<VisibleCellSpanBackgroundInfo>>();

            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();

            // section: 0 - Header, 1 - Body, 2 - Footer
            for (int rowSection = 0; rowSection < 3; rowSection++)
            {
                int firstRow, lastRow;
                ScrollRows.GetVisibleSection(rowSection, out firstRow, out lastRow);

                // section: 0 - Header, 1 - Body, 2 - Footer
                for (int columnSection = 0; columnSection < 3; columnSection++)
                {
                    int firstColumn, lastColumn;
                    ScrollColumns.GetVisibleSection(columnSection, out firstColumn, out lastColumn);

                    // Use dictionary to make sure each background is only added once.
                    BackgroundSpanDictionary backgroundsDictionary = new BackgroundSpanDictionary();

                    for (int visibleRowIndex = firstRow; visibleRowIndex <= lastRow; visibleRowIndex++)
                    {
                        VisibleLineInfo visibleRow = visibleRows[visibleRowIndex];

                        for (int visibleColumnIndex = firstColumn; visibleColumnIndex <= lastColumn; visibleColumnIndex++)
                        {
                            VisibleLineInfo visibleColumn = visibleColumns[visibleColumnIndex];
                            List<CellSpanBackgroundInfo> cellSpans = GetCellSpanBackgrounds(visibleRow.LineIndex, visibleColumn.LineIndex);

                            if (cellSpans != null)
                            {
                                foreach (CellSpanBackgroundInfo cellSpanBackgroundInfo in cellSpans)
                                {
                                    if (backgroundsDictionary.ContainsKey(cellSpanBackgroundInfo))
                                        continue;

                                    VisibleCellSpanBackgroundInfo ccSpan = new VisibleCellSpanBackgroundInfo();
                                    ccSpan.top = visibleRowIndex;
                                    ccSpan.left = visibleColumnIndex;
                                    ccSpan.cellSpan = cellSpanBackgroundInfo;
                                    ccSpan.RowSection = rowSection;
                                    ccSpan.ColumnSection = columnSection;

                                    backgroundsDictionary.Add(cellSpanBackgroundInfo, ccSpan);
                                    ccSpan.exactBounds = CellSpanToRect((ScrollAxisRegion)rowSection, (ScrollAxisRegion)columnSection, cellSpanBackgroundInfo);

                                    for (int nvRowIndex = visibleRowIndex; nvRowIndex <= lastRow; nvRowIndex++)
                                    {
                                        VisibleLineInfo nvRow = visibleRows[nvRowIndex];
                                        if (nvRow.LineIndex > cellSpanBackgroundInfo.Bottom)
                                            break;

                                        ccSpan.bottom = nvRowIndex;
                                        for (int nvColumnIndex = visibleColumnIndex; nvColumnIndex <= lastColumn; nvColumnIndex++)
                                        {
                                            VisibleLineInfo nvColumn = visibleColumns[nvColumnIndex];
                                            if (nvColumn.LineIndex > cellSpanBackgroundInfo.Right)
                                                break;

                                            ccSpan.right = nvColumnIndex;
                                        }

                                    }

                                    ccSpan.clippedBounds = GridUtil.FromLTRB(visibleColumn.ClippedOrigin, visibleRow.ClippedOrigin, visibleColumns[ccSpan.Right].ClippedCorner, visibleRows[ccSpan.Bottom].ClippedCorner);

                                }
                            }
                        }

                    }

                    // Sort found spans so that order how backgrounds are drawn (and thus z-order)
                    // is always the same and does not differ simply because of scroll position.
                    List<VisibleCellSpanBackgroundInfo> bgs = new List<VisibleCellSpanBackgroundInfo>();
                    bgs.AddRange(backgroundsDictionary.Values);
                    bgs.Sort();

                    framesListWithBackgoundSpans.Add(bgs);

                }
            }
        }

        /// <summary>
        /// Returns the rectangle for a cell span background clipped by the boundaries of the given
        /// row and column region.
        /// </summary>
        /// <param name="rowRegion">The row region.</param>
        /// <param name="columnRegion">The column region.</param>
        /// <param name="range">The cell span background range.</param>
        /// <returns></returns>
        public Rect CellSpanToRect(ScrollAxisRegion rowRegion, ScrollAxisRegion columnRegion, CellSpanInfo range)
        {
            DoubleSpan ySpan = ScrollRows.RangeToPoints(rowRegion, range.Top, range.Bottom, range.ClipRows);
            DoubleSpan xSpan = ScrollColumns.RangeToPoints(columnRegion, range.Left, range.Right, range.ClipColumns);

            if (ySpan.IsEmpty || xSpan.IsEmpty)
                return Rect.Empty;

            return new Rect(xSpan.Start, ySpan.Start, xSpan.Length, ySpan.Length);
        }

        public Rect CellSpanToClippedVisibleRect(CellSpanInfo rg)
        {
            DoubleSpan ySpan = ScrollRows.GetVisibleLinesClipPoints(rg.Top, rg.Bottom);
            DoubleSpan xSpan = ScrollColumns.GetVisibleLinesClipPoints(rg.Left, rg.Right);

            if (ySpan.IsEmpty || xSpan.IsEmpty)
                return Rect.Empty;

            return new Rect(xSpan.Start, ySpan.Start, xSpan.Length, ySpan.Length);
        }

        #endregion

        #region Virtualized UI Elements - ArrangedCellUIElements

        /// <summary>
        /// Provides routines for managing cells visuals (aka UIElement children)
        /// of rendered cells that have been associated with one or more UIElement
        /// visuals.
        /// </summary>
        public ArrangedCellUIElementsManager ArrangedCellUIElements
        {
            get { return arrangedCellUIElements; }
        }


        /// <summary>
        /// Clears the visuals for all cells and unloads or recycles the UIElement onjects. 
        /// Override this method to clear out additional cached information for the cells such
        /// as render style information. If UIElements belong to a virtualizing cell renderer 
        /// with AllowRecycle option enabled they will be moved to recycling bin. Otherwise
        /// they will be unloaded.
        /// </summary>
        public virtual void UnloadArrangedCells()
        {
            if(ArrangedCellUIElements !=null)
                ArrangedCellUIElements.UnloadAll();
#if !SILVERLIGHT
            if (RenderedCellVisuals!=null)
            RenderedCellVisuals.Invalidate();
#endif
            InvalidateVisual(true);
        }

        /// <summary>
        /// Calls <see cref="UnloadArrangedCells"/>.
        /// </summary>
        public void InvalidateCells()
        {
            UnloadArrangedCells();
        }

        /// <summary>
        /// Marks the visuals for a single cell to be reinitialized with 
        /// a call to <see cref="RefreshCellUIElementsContent"/> next time
        /// OnRender is called. Override this
        /// method to clear out additional cached information for the cell such
        /// as render style information.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        public virtual void InvalidateCell(RowColumnIndex cellRowColumnIndex)
        {
            ArrangedCellUIElements.Invalidate(cellRowColumnIndex);
#if !SILVERLIGHT
            RenderedCellVisuals.Invalidate(cellRowColumnIndex);
#endif
            InvalidateVisual(false);

            // will call RefreshCellUIElementsContent in OnRender if the cell
            // was not previously rearranged during a OnArrageContent 
            //RefreshCellUIElementsContent(cellRowColumnIndex);
        }

        /// <summary>
        /// Marks the visuals for a range of cells to be reinitialized with
        /// a call to <see cref="RefreshCellUIElementsContent"/> next time
        /// OnRender is called. Override this
        /// method to clear out additional cached information for the cell such
        /// as render style information.
        /// </summary>
        /// <param name="span">The range of cells.</param>
        public virtual void InvalidateCell(CellSpanInfoBase span)
        {
            InvalidateCell(span, false);
        }

        public virtual void InvalidateCell(CellSpanInfoBase span, bool dirtycelluielemet)
        {            
            bool visibleCellsAffected = ScrollRows.AnyVisibleLines(span.Top, span.Bottom) && ScrollColumns.AnyVisibleLines(span.Left, span.Right);

            if (!dirtycelluielemet)
                ArrangedCellUIElements.Invalidate(span);
            else
            {
                var scrollrows = ScrollRows.GetVisibleLines();
                var scrollcolumns = ScrollColumns.GetVisibleLines();
                if (scrollcolumns.Count > 0 && scrollrows.Count > 0)
                    arrangedCellUIElements.Invalidate(new CellSpanInfoBase(scrollrows[0].LineIndex, scrollcolumns.FirstBodyVisibleIndex, scrollrows.LastBodyVisibleIndex, scrollcolumns[scrollcolumns.Count - 1].LineIndex));
            }
#if !SILVERLIGHT
            RenderedCellVisuals.Invalidate(span);
#endif
            if (visibleCellsAffected)
                InvalidateVisual(false);
        }

        /// <summary>
        /// Gets the cell visuals for a cell.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <returns></returns>
        public CellUIElements GetCellUIElements(RowColumnIndex cellRowColumnIndex)
        {
            return arrangedCellUIElements.GetCellUIElements(cellRowColumnIndex);
        }

        /// <summary>
        /// Gets the cell visuals for a cell.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        public CellUIElements GetCellUIElements(int rowIndex, int columnIndex)
        {
            return GetCellUIElements(new RowColumnIndex(rowIndex, columnIndex));
        }
        #endregion

        #region ArrangeCellUIElements - create, position or unload UIElements inside cells

        /// <summary>
        /// Arranges the cells row by row. For each cell
        /// the virtual <see cref="OnArrangeCell"/> method is called. OnArrangeCell gets
        /// the <see cref="ICellRenderer"/> for a cell and calls its <see cref="ICellRenderer.Arrange"/>
        /// method.<para/>
        /// The method also implements the virtualization of UIElement children of cell renderers.
        /// It create new UIElement objects for cells scrolled into view or unload UIElements for 
        /// cells scrolled out of view. If a UIElement has focus whenscrolled out of view it will 
        /// be kept alive and not unloaded.
        /// </summary>
        /// <param name="arrangeSize"></param>
        protected virtual void ArrangeCellUIElements(Size arrangeSize)
        {

            // Create new UIElements for cells scrolled into view or unload UIElements for cells scrolled out of view.
            ArrangedCellUIElements.PrepareArrange();            
            Point corner = new Point(ScrollColumns.ViewCorner, ScrollRows.ViewCorner);
            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();

            visibleCoveredCellsArrangeId++;
            visibleOverlappingCellsArrangeId++;
            //visibleCellSpanBackgroundsArrangeId++;

            foreach (VisibleLineInfo visibleLine in visibleRows)
            {             
                ArrangeRow(visibleLine, corner);
            }
            ArrangedCellUIElements.ConcludeArrange();
        }

      

        void ArrangeRow(VisibleLineInfo visibleRow, Point corner)
        {
            int rowIndex = visibleRow.LineIndex;
            Rect cellRect = new Rect(0, visibleRow.Origin, ScrollColumns.ViewSize, visibleRow.Size);

            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();
            foreach (VisibleLineInfo visibleColumn in visibleColumns)
            {
                int columnIndex = visibleColumn.LineIndex;
                cellRect.X = visibleColumn.Origin;
                cellRect.Width = visibleColumn.Size;

                ArrangeCellArgs aca = new ArrangeCellArgs(this, visibleRow, visibleColumn, cellRect, InternalGetCellInfo(visibleRow, visibleColumn));


                VisibleOverlappingCellInfo icSpan = GetVisibleOverlappingCell(visibleRow, visibleColumn);
                if (icSpan != null)
                {
                    if (icSpan.arrangeId != visibleOverlappingCellsArrangeId || !icSpan.IsAmbiguousSection)
                    {
                        icSpan.arrangeId = visibleOverlappingCellsArrangeId;
                        aca.VisibleoverlappingCellInfo = icSpan;
                        aca.CellRect = icSpan.ExactBounds;
                        aca.VisibleoverlappingCellInfo.forceClipping = true;
                        aca.VisibleoverlappingCellInfo.CellSpan.ClipRows = true;
                        aca.VisibleoverlappingCellInfo.CellSpan.ClipColumns = true;
                        aca.IsProcessingArrangeCellUIElements = true;
                    }
                    ///Load the Image in InnerFrame.
                    aca.IsProcessingArrangeCellUIElements = true;
                    ArrangeCell(aca, this.InnerFrame);
                    continue;
                }

                VisibleCoveredCellInfo ccSpan = GetVisibleCoveredCell(visibleRow, visibleColumn);
                if (ccSpan != null)
                {
                    if (ccSpan.arrangeId == visibleCoveredCellsArrangeId)
                        continue;

                    if (ccSpan.IsAmbiguousSection)
                        continue;

                    if (hasReArranged && (ccSpan.CoveredCell.Top != visibleRow.LineIndex || ccSpan.CoveredCell.Left != visibleColumn.LineIndex))
                        continue;

                    ccSpan.arrangeId = visibleCoveredCellsArrangeId;
                    aca.VisibleCoveredCellInfo = ccSpan;
                    aca.CellRect = ccSpan.ExactBounds;
                }
                aca.IsProcessingArrangeCellUIElements = true;
                ArrangeCell(aca, this.ElementsFrame);
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="OnArrangeCell"/> is called.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in arrage cell; otherwise, <c>false</c>.
        /// </value>
        public bool IsInArrageCell
        {
            get
            {
                return this.isInArrage;
            }
        }

        void ArrangeCell(ArrangeCellArgs aca, VisualContainer frame)
        {
            VirtualizingCellsControlChildFrame canvas = GetChildFrame(aca, frame);

            ArrangedCellUIElements.PreArrangeCell(aca, canvas);

            ArrangeCellHelper(aca, canvas);
        }

        void ArrangeCellHelper(ArrangeCellArgs aca, VirtualizingCellsControlChildFrame canvas)
        {
            isInArrage = true;
            RowColumnIndex cellRowColumnIndex = aca.CellRowColumnIndex;

            try
            {
                List<UIElement> uiElements = aca.CellUIElements.UIElements;
                if (aca.ShouldCreateVisuals)
                {
                    PrepareCellUIElements(aca, uiElements, canvas);
                }

                foreach (UIElement el in uiElements)
                {
                    //Console.WriteLine(el.ToString());
                    // In WPF, we simply removed and reinserted the UIElement in the
                    // new canvas, but this causes glitches with Silverlight. It is better
                    // to simple create a new UIElement, initialize it. The old element
                    // gets hidden and recycled in PreArrangeCell.
                    VirtualizingCellsControlChildFrame oldCanvas = VisualTreeHelper.GetParent(el) as VirtualizingCellsControlChildFrame;
                    if (oldCanvas == null)
                        canvas.Children.Add(el);
                        //aca.CellUIElements.IsDirty = true;
                    //Console.WriteLine(aca.CellRowColumnIndex.ToString());

                }

                OnArrangeCell(aca);
            }
            finally
            {
                isInArrage = false;
            }

            ArrangedCellUIElements.PostArrangeCell(aca);
        }

        private VirtualizingCellsControlChildFrame GetChildFrame(CellArgs aca, VisualContainer frame)
        {
            bool isRowHeaderAtLeftSide = aca.IsRowHeaderAtLeftSide;
            bool isColumnHeaderAtTop = aca.IsColumnHeaderAtTop;
            bool isRowFooterAtRightSide = aca.IsRowFooterAtRightSide;
            bool isColumnFooterAtBottom = aca.IsColumnFooterAtBottom;

            if (aca.VisibleCoveredCellInfo != null)
            {
                VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(aca.VisibleCoveredCellInfo.CellSpan.Right);
                VisibleLineInfo visibleRow = ScrollRows.GetVisibleLineAtLineIndex(aca.VisibleCoveredCellInfo.CellSpan.Bottom);
                CellSpanInfo span = aca.VisibleCoveredCellInfo.CellSpan;

                if (aca.VisibleCoveredCellInfo.spansMultipleHorizontalSections
                    || visibleColumn == null && (isRowHeaderAtLeftSide || isRowFooterAtRightSide))
                //&& aca.VisibleCoveredCellInfo.CoveredCell.SpanWholeRow)
                {
                    DoubleSpan xSpan = ScrollColumns.GetVisibleLinesClipPoints(span.Left, span.Right);
                    aca.VisibleCoveredCellInfo.forceClipping |= true;// aca.VisibleCoveredCellInfo.ExactBounds.Width != xSpan.Length;
                    aca.VisibleCoveredCellInfo.clippedBounds.Width = xSpan.Length;
                    isRowHeaderAtLeftSide = true;
                    isRowFooterAtRightSide = true;
                }

                if (aca.VisibleCoveredCellInfo.spansMultipleVerticalSections
                    || visibleRow == null && (isColumnHeaderAtTop || isColumnFooterAtBottom))
                //&& aca.VisibleCoveredCellInfo.CoveredCell.SpanWholeColumn)
                {
                    DoubleSpan ySpan = ScrollRows.GetVisibleLinesClipPoints(span.Top, span.Bottom);
                    aca.VisibleCoveredCellInfo.forceClipping |= true;// aca.VisibleCoveredCellInfo.ExactBounds.Height != ySpan.Length;
                    aca.VisibleCoveredCellInfo.clippedBounds.Height = ySpan.Length;
                    isColumnHeaderAtTop = true;
                    isColumnFooterAtBottom = true;
                }
            }

            if (aca.VisibleoverlappingCellInfo != null)
            {
                VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(aca.VisibleoverlappingCellInfo.CellSpan.Right);
                VisibleLineInfo visibleRow = ScrollRows.GetVisibleLineAtLineIndex(aca.VisibleoverlappingCellInfo.CellSpan.Bottom);
                CellSpanInfo span = aca.VisibleoverlappingCellInfo.CellSpan;
                if (aca.VisibleoverlappingCellInfo.spansMultipleHorizontalSections
                   || visibleColumn == null && (isRowHeaderAtLeftSide || isRowFooterAtRightSide))
                {
                    DoubleSpan xSpan = ScrollColumns.GetVisibleLinesClipPoints(span.Left, span.Right);
                    aca.VisibleoverlappingCellInfo.forceClipping |= true;
                    aca.VisibleoverlappingCellInfo.clippedBounds.Width = xSpan.Length;
                    isRowHeaderAtLeftSide = true;
                    isRowFooterAtRightSide = true;
                }

                if (aca.VisibleoverlappingCellInfo.spansMultipleVerticalSections
                    || visibleRow == null && (isColumnHeaderAtTop || isColumnFooterAtBottom))
                {
                    DoubleSpan ySpan = ScrollRows.GetVisibleLinesClipPoints(span.Top, span.Bottom);
                    aca.VisibleoverlappingCellInfo.forceClipping |= true;
                    aca.VisibleoverlappingCellInfo.clippedBounds.Height = ySpan.Length;
                    isColumnHeaderAtTop = true;
                    isColumnFooterAtBottom = true;
                }
            }

            ScrollControlChildFrame canvas = GetChildFrame(isRowHeaderAtLeftSide, isColumnHeaderAtTop, isRowFooterAtRightSide, isColumnFooterAtBottom, frame);
            return (VirtualizingCellsControlChildFrame)canvas;
        }

        /// <summary>
        /// OnArrangeCell gets
        /// the <see cref="ICellRenderer"/> for a cell and calls its <see cref="ICellRenderer.Arrange"/>
        /// method. The method
        /// also adjust the <see cref="CellArgs.CellRect"/> and subtracts the border margins
        /// from the rectangle.
        /// </summary>
        /// <param name="aca">The cell layout information.</param>
        protected virtual void OnArrangeCell(ArrangeCellArgs aca)
        {
            aca.CellRect = aca.SubtractBorderMargins(aca.CellRect, aca.CellInfo.GetBorderMargins());

            if (aca.CellRect.IsEmpty)
                return;

            ICellRenderer renderer = GetCellRenderer(aca.CellInfo);
            if (renderer != null)
                renderer.Arrange(aca);
        }

        /// <summary>
        /// Prepares the cells UIElement children. The method gets the <see cref="ICellRenderer"/>
        /// for a cell and calls its <see cref="ICellRenderer.PrepareUIElements"/> method.
        /// </summary>
        /// <param name="aca">The cell layout information.</param>
        /// <param name="uiElements">The UI elements.</param>
        /// <param name="canvas">The child frame in the scroll control.</param>
        protected virtual void PrepareCellUIElements(ArrangeCellArgs aca, List<UIElement> uiElements, ScrollControlChildFrame canvas)
        {
            ICellRenderer renderer = GetCellRenderer(aca.CellInfo);
            if (renderer != null)
                renderer.PrepareUIElements(aca, uiElements, canvas);
        }

        #endregion

        #region ArrangeCellBorders - Calculate Borders. They will be rendered later in OnRender.
        List<CellBorderRangeList> verticalLines = new List<CellBorderRangeList>();
        List<CellBorderRangeList> horizontalLines = new List<CellBorderRangeList>();

        /// <summary>
        /// Calculates Cell Borders combining borders of neighbouring cells
        /// with same value - they will be rendered later in OnRender.
        /// </summary>
        /// <param name="arrangeSize"></param>
        protected virtual void ArrangeCellBorders(Size arrangeSize)
        {
            bordersDirty = false;
            needRenderBorders = true;

            // Calculate Borders - they will be rendered later in OnRender.
            int first, last;
            verticalLines.Clear();
            horizontalLines.Clear();            

            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();

            // For each column, check neighbouring cells with same border 
            foreach (VisibleLineInfo visibleColumn in visibleColumns)
            {
                // side: 0 - left border, 1 - right border
                for (int side = 0; side < 2; side++)
                {
                    CellBorderSide borderSide = side == 0 ? CellBorderSide.Left : CellBorderSide.Right;
                    CellBorderRangeList innerBorders = new CellBorderRangeList(visibleColumn, borderSide);
                    verticalLines.Add(innerBorders);

                    // section: 0 - Header, 1 - Body, 2 - Footer
                    for (int section = 0; section < 3; section++)
                    {
                        CellBorderRange borderRange = null;
                        IRenderCellInfo previousCellInfo = null;
                        Pen previousBorder = null;
                        ScrollRows.GetVisibleSection(section, out first, out last);
                        for (int visibleIndex = first; visibleIndex <= last; visibleIndex++)
                        {
                            VisibleLineInfo visibleRow = visibleRows[visibleIndex];
                            IRenderCellInfo ci = InternalGetCellInfo(visibleRow, visibleColumn);
                            Pen border = InternalGetCellBorder(visibleRow, visibleColumn, ci, borderSide);

                            #region Overlapping cells implementation
                            foreach (KeyValuePair<RowColumnIndex, int> val in floatcellran)
                            {
                                for (int i = val.Key.ColumnIndex; i < val.Value; i++)
                                {
                                    if (visibleRow.LineIndex == val.Key.RowIndex && visibleColumn.LineIndex == i && borderSide == CellBorderSide.Right)
                                        border = null;
                                }
                            }
                            #endregion

                            if (borderRange != null)
                            {
                                if (CanCombineCellBorder(previousCellInfo, ci, previousBorder, border, borderSide))
                                {
                                    borderRange.last = visibleIndex;
                                    continue;
                                }

                                innerBorders.Add(borderRange);
                            }

                            if (border != null)
                                borderRange = new CellBorderRange(border, visibleIndex);
                            else
                                borderRange = null;

                            previousCellInfo = ci;
                            previousBorder = border;
                        }
                        if (borderRange != null)
                        {
                            borderRange.last = last;                           
                            innerBorders.Add(borderRange);
                        }
                    }
                }
            }

            // For each row, check neighbouring cells with same border 
            foreach (VisibleLineInfo visibleRow in visibleRows)
            {
                // side: 0 - top border, 1 - bottom border
                for (int side = 0; side < 2; side++)
                {
                    CellBorderSide borderSide = side == 0 ? CellBorderSide.Top : CellBorderSide.Bottom;
                    CellBorderRangeList innerBorders = new CellBorderRangeList(visibleRow, borderSide);
                    horizontalLines.Add(innerBorders);

                    // section: 0 - Header, 1 - Body, 2 - Footer
                    for (int section = 0; section < 3; section++)
                    {
                        CellBorderRange borderRange = null;
                        IRenderCellInfo previousCellInfo = null;
                        Pen previousBorder = null;
                        ScrollColumns.GetVisibleSection(section, out first, out last);
                        for (int visibleIndex = first; visibleIndex <= last; visibleIndex++)
                        {
                            VisibleLineInfo visibleColumn = visibleColumns[visibleIndex];
                            IRenderCellInfo ci = InternalGetCellInfo(visibleRow, visibleColumn);
                            Pen border = InternalGetCellBorder(visibleRow, visibleColumn, ci, borderSide);
                            if (borderRange != null)
                            {
                                if (CanCombineCellBorder(previousCellInfo, ci, previousBorder, border, borderSide))
                                {
                                    borderRange.last = visibleIndex;
                                    continue;
                                }

                                innerBorders.Add(borderRange);
                            }

                            if (border != null)
                                borderRange = new CellBorderRange(border, visibleIndex);
                            else
                                borderRange = null;

                            previousCellInfo = ci;
                            previousBorder = border;
                        }
                        if (borderRange != null)
                        {
                            borderRange.last = last;                            
                            innerBorders.Add(borderRange);
                        }
                    }
                }
            }
        }

        protected virtual bool CanCombineCellBorder(IRenderCellInfo previousCellInfo, IRenderCellInfo ci, Pen previousBorder, Pen border, CellBorderSide borderSide)
        {
            return previousBorder.Equals(border);
        }

        Pen InternalGetCellBorder(VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn, IRenderCellInfo ci, CellBorderSide borderSide)
        {
            CoveredCellInfo cci = GetCoveredCell(visibleRow, visibleColumn);
            if (cci != null)
            {
                switch (borderSide)
                {
                    case CellBorderSide.Top:
                        if (cci.Top != visibleRow.LineIndex)
                            return null;
                        break;
                    case CellBorderSide.Left:
                        if (cci.Left != visibleColumn.LineIndex)
                            return null;
                        break;
                    case CellBorderSide.Bottom:
                        if (cci.Bottom != visibleRow.LineIndex)
                            return null;
                        break;
                    case CellBorderSide.Right:
                        if (cci.Right != visibleColumn.LineIndex)
                            return null;
                        break;
                }
            }

            return GetCellBorder(ci, borderSide);
        }

        /// <summary>
        /// Gets the cell border from the given cell style.
        /// </summary>
        /// <param name="ci">The cell style.</param>
        /// <param name="borderSide">The border side.</param>
        /// <returns></returns>
        protected virtual Pen GetCellBorder(IRenderCellInfo ci, CellBorderSide borderSide)
        {
            // If border stored in CellInfo is not a pen and instead a description of a pen (e.g. GridBorder)
            // this method is a good place to convert that to a pen and cache it for reuse in other cells.
            return ci.GetCellBorder(borderSide) as Pen;
        }

        #endregion

        #endregion

        #region Create UIElement when hovering mouse

        //void previewMouseMove(object sender, MouseEventArgs e)
        //{
        //    DelayedCreateCellUIElements(GetPosition(e));
        //}

        /// <summary>
        /// Gets the visible row and visible column for a given cell which can also be a covered cell.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="visibleRow">The visible row.</param>
        /// <param name="visibleColumn">The visible column.</param>
        public void GetVisibleRowAndColumn(RowColumnIndex cellRowColumnIndex, out VisibleLineInfo visibleRow, out VisibleLineInfo visibleColumn)
        {
            visibleRow = ScrollRows.GetVisibleLineAtLineIndex(cellRowColumnIndex.RowIndex);
            visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(cellRowColumnIndex.ColumnIndex);
            if (visibleRow == null || visibleColumn == null)
            {
                CoveredCellInfo cc = GetCoveredCell(cellRowColumnIndex);
                if (cc == null)
                    return;

                if (visibleRow == null)
                {
                    foreach (VisibleLineInfo row in ScrollRows.GetVisibleLines())
                        if (row.LineIndex >= cc.Top && row.LineIndex <= cc.Bottom)
                        {
                            visibleRow = row;
                            break;
                        }
                }

                if (visibleColumn == null)
                {
                    foreach (VisibleLineInfo column in ScrollColumns.GetVisibleLines())
                        if (column.LineIndex >= cc.Left && column.LineIndex <= cc.Right)
                        {
                            visibleColumn = column;
                            break;
                        }
                }
            }
        }

        ArrangeCellArgs PrepareCreateOrRefreshCellUIElements(RowColumnIndex cellRowColumnIndex)
        {
            if (cellRowColumnIndex.IsEmpty)
                return null;

            CellUIElements visuals;

            VisibleLineInfo visibleRow;
            VisibleLineInfo visibleColumn;
            VisibleCoveredCellInfo ccSpan;
            VisibleOverlappingCellInfo icSpan;
            Rect cellRect;
            GetVisibleRowAndColumn(cellRowColumnIndex, out visibleRow, out visibleColumn);
            if (visibleRow == null || visibleColumn == null)
            {
                CoveredCellInfo cc = GetCoveredCell(cellRowColumnIndex);
                if (cc != null)
                    cellRowColumnIndex = new RowColumnIndex(cc.Top, cc.Left);
                OverlappingCellInfo ic = GetOverlappingCell(cellRowColumnIndex);
                if (ic != null)
                    cellRowColumnIndex = new RowColumnIndex(ic.Top, ic.Left);
                visibleRow = ScrollRows.GetVisibleLineAtLineIndex(cellRowColumnIndex.RowIndex, true);
                visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(cellRowColumnIndex.ColumnIndex, true);
                cellRect = new Rect(0, 0, 0, 0); // Rect.Empty causes issues in OnArrangeCell where rect is check for .IsEmpty.
                ccSpan = null;
                icSpan = null;
            }
            else
            {
                cellRect = GridUtil.FromLTRB(visibleColumn.Origin, visibleRow.Origin, visibleColumn.Corner, visibleRow.Corner);
                ccSpan = GetVisibleCoveredCell(visibleRow, visibleColumn);
                icSpan = GetVisibleOverlappingCell(visibleRow, visibleColumn);
                if (ccSpan != null)
                {
                    if (ccSpan.IsAmbiguousSection)
                        return null;

                    cellRowColumnIndex = new RowColumnIndex(ccSpan.CoveredCell.Top, ccSpan.CoveredCell.Left);
                    visibleRow = ScrollRows.GetVisibleLines()[ccSpan.Top];
                    visibleColumn = ScrollColumns.GetVisibleLines()[ccSpan.Left];
                    cellRect = ccSpan.ExactBounds;
                }
                if (icSpan != null)
                {
                    if (icSpan.IsAmbiguousSection)
                        return null;
                    cellRowColumnIndex = new RowColumnIndex(icSpan.OverlappingCell.Top, icSpan.OverlappingCell.Left);
                    visibleRow = ScrollRows.GetVisibleLines()[icSpan.Top];
                    visibleColumn = ScrollColumns.GetVisibleLines()[icSpan.Left];
                    cellRect = icSpan.ExactBounds;
                }
            }
            visuals = GetCellUIElements(cellRowColumnIndex);

            IRenderCellInfo ci = GetRenderCellInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
            ArrangeCellArgs aca = new ArrangeCellArgs(this, visibleRow, visibleColumn, cellRect, GetRenderCellInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex));
            aca.VisibleCoveredCellInfo = ccSpan;
            aca.VisibleoverlappingCellInfo = icSpan;
            aca.SetCellUIElements(visuals);
            return aca;
        }

        /// <summary>
        /// Determine if cell at given cells row and column index is associated with UIElement. If not,
        /// check if UIElement can be created.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        public bool DelayedCreateCellUIElements(RowColumnIndex cellRowColumnIndex)
        {
            ArrangeCellArgs aca = PrepareCreateOrRefreshCellUIElements(cellRowColumnIndex);
            if (aca == null || !aca.VisibleColumn.IsVisible || !aca.VisibleRow.IsVisible)
                return false;

            if (aca.HasVisuals)
                return true;

            aca.ShouldCreateVisuals = true;
            VirtualizingCellsControlChildFrame canvas = GetChildFrame(aca, this.ElementsFrame);
            ArrangeCellHelper(aca, canvas);
            if (aca.HasVisuals)
                EraseRenderedCell(aca.CellRowColumnIndex, aca.OriginalCellRect);
            return true;
        }

        /// <summary>
        /// Determine if cell at given cells row and column index is associated with UIElement. If yes,
        /// reintialize the cells UIElement with a call to <see cref="OnArrangeCell"/>. OnArrangeCell gets
        /// the <see cref="ICellRenderer"/> for a cell and calls its <see cref="ICellRenderer.Arrange"/>
        /// method. This usually is called as a response to an earlier <see cref="InvalidateCell"/>
        /// call.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        public void RefreshCellUIElementsContent(RowColumnIndex cellRowColumnIndex)
        {
            ArrangeCellArgs aca = PrepareCreateOrRefreshCellUIElements(cellRowColumnIndex);
            if (aca == null || !aca.HasVisuals)
                return;

            if (aca.VisibleoverlappingCellInfo == null)
            {
                aca.ShouldReinitializeContent = true;
                VirtualizingCellsControlChildFrame canvas = GetChildFrame(aca, this.ElementsFrame);
                ArrangeCellHelper(aca, canvas);
            }
        }

        /// <summary>
        /// Remove previously rendered cell from the <see cref="ScrollControl.ForegroundFrame"/> frame. This is needed
        /// when a cells UIElement where created because the previous rendering (without UIElements
        /// being created like in an optimized TextBox renderer) would otherwise draw over the new
        /// arranged elements.
        /// </summary>
        /// <param name="cellRowColumnIndex">The cells row and column index.</param>
        /// <param name="cellRect">The cells bounds.</param>
        public void EraseRenderedCell(RowColumnIndex cellRowColumnIndex, Rect cellRect)
        {
            if (!useDrawingVisualForCells)
            {
                Geometry clipGeometry = foregroundFrameCellsVisual.Clip;
                if (clipGeometry == null)
                    clipGeometry = new RectangleGeometry(new Rect(0, 0, RenderSize.Width, RenderSize.Height));
                clipGeometry = Geometry.Combine(clipGeometry, new RectangleGeometry(cellRect), GeometryCombineMode.Exclude, null);

                foregroundFrameCellsVisual.Clip = clipGeometry;
            }
            else
            {
                foreach (Visual visual in InnerFrame.Children)
                {
                    VirtualizingCellsControlChildFrame frame = visual as VirtualizingCellsControlChildFrame;
                    if (frame != null)
                    {
                        // I could remove the DrawingVisual here with
                        // frame.RenderedCells.aliveVisuals.Remove(cellRowColumnIndex);
                        // This will however cause issues if a CellRenderer mixes both rendered content with arranged UIElements.
                        // As a future optimization I could add a flag to CellRenderer indicating whether it wants to mix content.
                        //
                        // The save bet is to invalidate the cell and force it to Rerender immediately. The cell renderer
                        // can then simply leave the DrawingVisual blank. 
                        if (frame.RenderedCells.Invalidate(cellRowColumnIndex))
                        {
                            VisibleLineInfo visibleRow = ScrollRows.GetVisibleLineAtLineIndex(cellRowColumnIndex.RowIndex);
                            VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(cellRowColumnIndex.ColumnIndex);
                            if (visibleRow != null && visibleColumn != null)
                            {
                                RenderCellArgs rca = new RenderCellArgs(this, visibleRow, visibleColumn, cellRect, InternalGetCellInfo(visibleRow, visibleColumn));
                                RenderCell(null, rca);
                            }
                        }
                    }
                }

            }
        }
        #endregion

        #region PointToCellRowColumnIndex
        /// <summary>
        /// Determines the cell under the mouse location.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="adjustForCoveredCells">if set to <c>true</c> adjust cell index if inside a covered cells to return the top and left index of the covered cell.</param>
        /// <returns>
        /// The cells row and column index under the mouse location.
        /// </returns>
        public RowColumnIndex PointToCellRowColumnIndex(MouseEventArgs e, bool adjustForCoveredCells)
        {
            return PointToCellRowColumnIndex(e.GetPosition(this), adjustForCoveredCells);
        }

        /// <summary>
        /// Determines the cell under the mouse location.
        /// </summary>
        /// <param name="p">The point in client coordinates.</param>
        /// <param name="adjustForCoveredCells">if set to <c>true</c> adjust cell index if inside a covered cells to return the top and left index of the covered cell.</param>
        /// <returns>
        /// The cells row and column index under the mouse location.
        /// </returns>
        public RowColumnIndex PointToCellRowColumnIndex(Point p, bool adjustForCoveredCells)
        {
            VisibleLineInfo visibleRow = ScrollRows.GetVisibleLineAtPoint(p.Y);
            VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLineAtPoint(p.X);

            if (visibleRow == null || visibleColumn == null)
                return RowColumnIndex.Empty;

            // If cells belongs to a covered cell, return top and left position where covered cell starts.
            if (adjustForCoveredCells)
            {
                CoveredCellInfo cci = GetCoveredCell(visibleRow, visibleColumn);
                if (cci != null)
                    return new RowColumnIndex(cci.Top, cci.Left);

                OverlappingCellInfo ic = GetOverlappingCell(visibleRow, visibleColumn);
                if (ic != null)
                    return new RowColumnIndex(ic.Top, ic.Left);
            }

            return new RowColumnIndex(visibleRow.LineIndex, visibleColumn.LineIndex);
        }

        /// <summary>
        /// Adjusts the index of the cell index if inside a covered cell
        /// to return the top and left index of the covered cell. If not
        /// inside a covered cell returns the original cell index.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <returns></returns>
        public RowColumnIndex AdjustCoveredCellRowColumnIndex(RowColumnIndex cellRowColumnIndex)
        {
            CoveredCellInfo cci = GetCoveredCell(cellRowColumnIndex);
            if (cci != null)
                return new RowColumnIndex(cci.Top, cci.Left);
            return cellRowColumnIndex;
        }
        #endregion

        #region GetCellInfo

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
        protected internal abstract IRenderCellInfo GetRenderCellInfo(int rowIndex, int columnIndex);

        /// <summary>
        /// Gets the cell renderer from a render cell style.
        /// </summary>
        /// <param name="cellInfo">The render cell style.</param>
        /// <returns></returns>
        protected internal abstract ICellRenderer GetCellRenderer(IRenderCellInfo cellInfo);

        protected IRenderCellInfo InternalGetCellInfo(VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn)
        {
            if (coveredCellsProvider != null && !coveredCellsProvider.IsEmpty)
            {
                CoveredCellInfo coveredCell = GetCoveredCell(visibleRow, visibleColumn);
                if (coveredCell != null)
                    return InternalGetCellInfo(coveredCell);
            }

            return GetRenderCellInfo(visibleRow.LineIndex, visibleColumn.LineIndex);
        }

        protected IRenderCellInfo InternalGetCellInfo(CoveredCellInfo coveredCellInfo)
        {
            return GetRenderCellInfo(coveredCellInfo.Top, coveredCellInfo.Left);
        }

        #endregion

        #region Private CellBorderRangeList, CellBorderRange classes
        class CellBorderRangeList : List<CellBorderRange> , IDisposable
        {
            public CellBorderRangeList(VisibleLineInfo visibleLine, CellBorderSide borderSide)
            {
                this.visibleLine = visibleLine;
                this.borderSide = borderSide;
            }
            
            internal VisibleLineInfo visibleLine;
            internal CellBorderSide borderSide;

            public void Dispose()
            {
              if(this.visibleLine!=null)
                this.visibleLine = null;
              
            }
        }

        class CellBorderRange
        {
            public CellBorderRange(Pen border, int first)
            {
                this.border = border;
                this.first = first;
                this.last = first;
            }

            internal Pen border;
            internal int first;
            internal int last;            

            public override string ToString()
            {
                return String.Format("CellBorderRange: {0}->{1}", first, last);
            }
        }
        #endregion

        #region Private VisibleCombinedCellBackgroundInfo class

        class VisibleCombinedCellBackgroundInfo : VisibleCellSpanInfo
        {
            public VisibleCombinedCellBackgroundInfo(int top, int left, Brush background)
                : base(top, left)
            {
                this.background = background;
            }

            internal Brush background;

            public override string ToString()
            {
                return String.Format("CellBackgroundSpan: {0},{1},{2},{3},{4}", top, left, bottom, right, background);
            }
        }
        #endregion

        #region Private VisibleCellSpanLayout class, GetCellSpanDelegate delegate
        delegate CellSpanInfo GetCellSpanDelegate(int rowIndex, int columnIndex);

        class VisibleCellSpanLayout<T> : IEnumerable<T>
            where T : VisibleCellSpanInfo, new()
        {
            VirtualizingCellsControl _cellsControl;
            List<T> visibleCellSpansList = new List<T>();
            List<List<T>> visibleCellSpansTable = new List<List<T>>();
            GetCellSpanDelegate getCellSpan;
            bool isDirty;

            public VisibleCellSpanLayout(VirtualizingCellsControl cellsControl, GetCellSpanDelegate getCellSpan)
            {
                this._cellsControl = cellsControl;
                this.getCellSpan = getCellSpan;
            }

            public void SetDirty()
            {
                if (_cellsControl.isInArrage)
                    throw new InvalidOperationException();
                if (visibleCellSpansList.Count > 0)
                    isDirty = true;
            }

            public ScrollAxisBase ScrollRows
            {
                get
                {
                    return _cellsControl.ScrollRows;
                }
            }

            public ScrollAxisBase ScrollColumns
            {
                get
                {
                    return _cellsControl.ScrollColumns;
                }
            }

            /// <summary>
            /// Allocates a grid of integer values and ensures enough entries
            /// are there for each visible row and column.
            /// </summary>
            public void PrepareVisibleCellSpansTable()
            {
                VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
                VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();
                int c = visibleColumns.Count;
                int r = visibleRows.Count;

                for (int n = 0; n < visibleCellSpansTable.Count; n++)
                    PrepareVisibleCellSpansTableRow(visibleCellSpansTable[n], c);

                while (visibleCellSpansTable.Count < r)
                {
                    List<T> colBits = new List<T>();
                    PrepareVisibleCellSpansTableRow(colBits, c);
                    visibleCellSpansTable.Add(colBits);
                }
            }

            public void PrepareVisibleCellSpansTableRow(List<T> cells, int count)
            {
                for (int n = 0; n < cells.Count; n++)
                    cells[n] = null;

                while (cells.Count < count)
                    cells.Add(null);
            }

            void EnsureClean()
            {
                if (isDirty)
                    ArrangeCellSpans(this._cellsControl.RenderSize, false);
            }

            public void ArrangeCellSpans(Size arrangeSize, bool isEmpty)
            {
                isDirty = false;
                visibleCellSpansList.Clear();
                if (isEmpty)
                {
                    visibleCellSpansTable.Clear();
                    return;
                }
                PrepareVisibleCellSpansTable();

                VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
                VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();
                
                // section: 0 - Header, 1 - Body, 2 - Footer
                for (int rowSection = 0; rowSection < 3; rowSection++)
                {
                    int firstRow, lastRow;
                    ScrollRows.GetVisibleSection(rowSection, out firstRow, out lastRow);

                    for (int visibleRowIndex = firstRow; visibleRowIndex <= lastRow; visibleRowIndex++)
                    {
                        VisibleLineInfo visibleRow = visibleRows[visibleRowIndex];

                        // section: 0 - Header, 1 - Body, 2 - Footer
                        for (int columnSection = 0; columnSection < 3; columnSection++)
                        {
                            int firstColumn, lastColumn;
                            ScrollColumns.GetVisibleSection(columnSection, out firstColumn, out lastColumn);

                            for (int visibleColumnIndex = firstColumn; visibleColumnIndex <= lastColumn; visibleColumnIndex++)
                            {

                                T foundSpan = visibleCellSpansTable[visibleRowIndex][visibleColumnIndex];
                                // Skip this if cell belongs to an CellSpan from a previous row.
                                if (foundSpan != null)
                                {
                                    visibleColumnIndex = foundSpan.Right;
                                    continue;
                                }

                                VisibleLineInfo visibleColumn = visibleColumns[visibleColumnIndex];
                                CellSpanInfo cellSpanBackgroundInfo = GetCellSpan(visibleRow.LineIndex, visibleColumn.LineIndex);

                                if (cellSpanBackgroundInfo != null)
                                {
                                    bool isAmbiguous = false;
                                    if (visibleRow.VisibleIndex > 0 && visibleRow.LineIndex != cellSpanBackgroundInfo.Top)
                                    {
                                        T otherSpan = GetVisibleCellSpan(ScrollRows.GetVisibleLines()[visibleRow.VisibleIndex - 1], visibleColumn);

                                        if (otherSpan != null && otherSpan.CellSpan.Equals(cellSpanBackgroundInfo))
                                        {
                                            if (!otherSpan.isAmbiguousSection)
                                                otherSpan.spansMultipleVerticalSections = true;
                                            isAmbiguous = true;
                                        }
                                    }

                                    if (visibleColumn.VisibleIndex > 0 && visibleColumn.LineIndex != cellSpanBackgroundInfo.Left)
                                    {
                                        T otherSpan = GetVisibleCellSpan(visibleRow, ScrollColumns.GetVisibleLines()[visibleColumn.VisibleIndex - 1]);

                                        if (otherSpan != null && otherSpan.CellSpan.Equals(cellSpanBackgroundInfo))
                                        {
                                            if (!otherSpan.isAmbiguousSection)
                                                otherSpan.spansMultipleHorizontalSections = true;
                                            isAmbiguous = true;
                                        }
                                    }

                                    T ccSpan = new T();
                                    ccSpan.top = visibleRowIndex;
                                    ccSpan.left = visibleColumnIndex;
                                    ccSpan.cellSpan = cellSpanBackgroundInfo;

                                    visibleCellSpansList.Add(ccSpan);
                                    ccSpan.exactBounds = CellSpanToRect((ScrollAxisRegion)rowSection, (ScrollAxisRegion)columnSection, cellSpanBackgroundInfo);
                                    ccSpan.isAmbiguousSection = isAmbiguous;

                                    for (int nvRowIndex = visibleRowIndex; nvRowIndex <= lastRow; nvRowIndex++)
                                    {
                                        VisibleLineInfo nvRow = visibleRows[nvRowIndex];
                                        if (nvRow.LineIndex > cellSpanBackgroundInfo.Bottom)
                                            break;

                                        ccSpan.bottom = nvRowIndex;
                                        for (int nvColumnIndex = visibleColumnIndex; nvColumnIndex <= lastColumn; nvColumnIndex++)
                                        {
                                            VisibleLineInfo nvColumn = visibleColumns[nvColumnIndex];
                                            if (nvColumn.LineIndex > cellSpanBackgroundInfo.Right)
                                                break;

                                            visibleCellSpansTable[nvRowIndex][nvColumnIndex] = ccSpan;
                                            ccSpan.right = nvColumnIndex;
                                        }

                                    }

                                    ccSpan.clippedBounds = GridUtil.FromLTRB(visibleColumn.ClippedOrigin, visibleRow.ClippedOrigin, visibleColumns[ccSpan.Right].ClippedCorner, visibleRows[ccSpan.Bottom].ClippedCorner);

                                }
                            }

                        }
                    }
                }
            }

            public CellSpanInfo GetCellSpan(int rowIndex, int columnIndex)
            {
                return getCellSpan(rowIndex, columnIndex);
            }


            public Rect CellSpanToRect(ScrollAxisRegion rowRegion, ScrollAxisRegion columnRegion, CellSpanInfo range)
            {
                DoubleSpan ySpan = ScrollRows.RangeToPoints(rowRegion, range.Top, range.Bottom, range.ClipRows);
                DoubleSpan xSpan = ScrollColumns.RangeToPoints(columnRegion, range.Left, range.Right, range.ClipColumns);

                if (ySpan.IsEmpty || xSpan.IsEmpty)
                    return Rect.Empty;

                return new Rect(xSpan.Start, ySpan.Start, xSpan.Length, ySpan.Length);
            }

            public T GetVisibleCellSpan(VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn)
            {
                EnsureClean();

                if (visibleRow.VisibleIndex < visibleCellSpansTable.Count
                    && visibleColumn.VisibleIndex < visibleCellSpansTable[visibleRow.VisibleIndex].Count)
                {
                    T ccSpan = visibleCellSpansTable[visibleRow.VisibleIndex][visibleColumn.VisibleIndex];
                    return ccSpan;
                }
                return null;
            }

            public CellSpanInfo GetCellSpan(VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn)
            {
                EnsureClean();

                if (visibleRow.VisibleIndex < visibleCellSpansTable.Count
                    && visibleColumn.VisibleIndex < visibleCellSpansTable[visibleRow.VisibleIndex].Count)
                {
                    T ccSpan = visibleCellSpansTable[visibleRow.VisibleIndex][visibleColumn.VisibleIndex];
                    if (ccSpan != null)
                        return ccSpan.CellSpan;
                }
                return null;
            }


            #region IEnumerable Members

            public IEnumerator GetEnumerator()
            {
                EnsureClean();
                return ((IEnumerable)this.visibleCellSpansList).GetEnumerator();
            }

            #endregion

            #region IEnumerable<T> Members

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                EnsureClean();
                return this.visibleCellSpansList.GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region Private BackgroundSpanDictionary class
        class BackgroundSpanDictionary : Dictionary<CellSpanBackgroundInfo, VisibleCellSpanBackgroundInfo>
        {
        }
        #endregion

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.horizontalLines = null;
                this.verticalLines = null;
                this.coveredCellsLayout = null;
                this.cellBackgroundDrawingVisuals = null;
                this.cellBackgroundRenderStyleDrawingVisuals = null;
                this.backgroundFrameBorderVisual = null;
                this.foregroundFrameCellsVisual = null;
                //if (this.ScrollColumns != null)
                //    this.ScrollColumns.Dispose(true);
                //if (this.ScrollRows != null)
                //    this.ScrollRows.Dispose(true);
                this.renderedCells = null;
                this.backgroundFrameBorders = null;
                if (this.combinedCellBackgroundsList != null)
                {
                    this.combinedCellBackgroundsList.Clear();
                    this.combinedCellBackgroundsList = null;
                }
                if (this.combinedCellBackgroundIdsTable != null)
                {
                    this.combinedCellBackgroundIdsTable.Clear();
                    this.combinedCellBackgroundIdsTable = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}