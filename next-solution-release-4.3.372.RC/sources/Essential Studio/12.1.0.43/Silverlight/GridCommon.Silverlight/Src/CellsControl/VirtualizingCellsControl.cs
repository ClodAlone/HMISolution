#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
//#define TestDrawTextPerformance
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
#if SILVERLIGHT
using VirtualizingCellsControlChildFrame = Syncfusion.Windows.Controls.Scroll.ScrollControlChildFrame;
#endif

#if !WinRT

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using VirtualizingCellsControlChildFrame = Syncfusion.WinRT.Controls.Scroll.ScrollControlChildFrame;

namespace Syncfusion.WinRT.Controls.Cells
#endif
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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public abstract class VirtualizingCellsControl : ScrollAxisControl
    {
        #region Fields
#if (!SILVERLIGHT && !WinRT)
        // MouseController
        CellMouseControllerDispatcher mouseControllerDispatcher = null;

        RenderedCellsManager renderedCells;

        // DrawingVisuals
        bool useDrawingVisualForCells = true;
        bool isRenderCellBackgroundPhase = false;
        bool isInOnRender = false;

        DrawingVisual foregroundFrameCellsVisual = new NoHitTestDrawingVisual();
        DrawingVisual foregroundFrameBorderVisual = new NoHitTestDrawingVisual();
        DrawingVisual cellBackgroundDrawingVisuals = new NoHitTestDrawingVisual();
        DrawingVisual cellBackgroundRenderStyleDrawingVisuals = new NoHitTestDrawingVisual();

        bool clearVisualsCacheWhenUnloaded = false;
#else
        Panel cellBackgrounds = new VisualContainer("CellBackgrounds");
        Panel foregroundFrameBorders = new VisualContainer("CellBorders");
#endif

        // Virtualized UI Elements
        ArrangedCellUIElementsManager arrangedCellUIElements;

        bool needRenderBorders = true; // set in ArrangeBorders
        bool needRenderBackgrounds = true; // set in ArrangeCombinedCellBackgrounds
        //bool needRenderStyleBackgrounds = true; // set this when PrepareRenderCellInfo can change background.

        // ArrangeCombinedCellBackgrounds
        List<VisibleCombinedCellBackgroundInfo> combinedCellBackgroundsList = new List<VisibleCombinedCellBackgroundInfo>();
        List<List<int>> combinedCellBackgroundIdsTable = new List<List<int>>();
        int combinedCellBackgroundArrangeId = 1;
        //bool backgroundsDirty = true;
        //bool bordersDirty = true;

        // Covered Cells
        ICoveredCellsProvider coveredCellsProvider;
        IOverlappingCellProvider overlappingCellsProvider;
        VisibleCellSpanLayout<VisibleCoveredCellInfo> coveredCellsLayout;
        VisibleCellSpanLayout<VisibleOverlappingCellInfo> overlappingCellsLayout;
        int visibleCoveredCellsArrangeId = 0;
        int visibleOverlappingCellsArrangeId = 0;

        // CellSpanBackgrounds
        ICellSpanBackgroundsProvider cellSpanBackgroundsProvider;
        List<List<VisibleCellSpanBackgroundInfo>> framesListWithBackgoundSpans;

        // Helper
        bool isInArrage;


        #endregion
#if (!SILVERLIGHT && !WinRT)
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
                RenderedCellVisuals.Invalidate();
                ArrangedCellUIElements.UnloadAll();

                combinedCellBackgroundsList.Clear();
                combinedCellBackgroundIdsTable.Clear();
            }

            base.OnUnloaded(e);
        }
#endif
        #region Dependency Properties

        #region CellsControlProperty

        /// <summary>
        /// Returns the <see cref="VirtualizingCellsControl"/> of an UIElement inside a cell. When the editor
        /// inside a cell has children and you query this for a child it will query the top-most 
        /// parent element of the cell renderer for the value of the property.
        /// </summary>
        public static readonly DependencyProperty CellsControlProperty = DependencyProperty.RegisterAttached(
        "CellsControl", typeof(VirtualizingCellsControl), typeof(VirtualizingCellsControl), new PropertyMetadata(null));

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
        public static readonly DependencyProperty CellUIElementProperty = DependencyProperty.RegisterAttached(
            "CellUIElement", typeof(UIElement), typeof(VirtualizingCellsControl), new PropertyMetadata(null));

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
        public static readonly DependencyProperty ArrangeCellArgsProperty = DependencyProperty.RegisterAttached(
            "ArrangeCellArgsEvent", typeof(ArrangeCellArgs), typeof(VirtualizingCellsControl), new PropertyMetadata(null));

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
        public static readonly DependencyProperty CellRowColumnIndexProperty = DependencyProperty.RegisterAttached(
            "CellRowColumnIndex", typeof(RowColumnIndex), typeof(VirtualizingCellsControl), new PropertyMetadata(RowColumnIndex.Empty));

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
        public static readonly DependencyProperty RenderCellInfoProperty = DependencyProperty.RegisterAttached(
            "RenderCellInfo", typeof(IRenderCellInfo), typeof(VirtualizingCellsControl), new PropertyMetadata(null));

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
        public static readonly DependencyProperty CellRendererProperty = DependencyProperty.RegisterAttached(
            "CellRenderer", typeof(ICellRenderer), typeof(VirtualizingCellsControl), new PropertyMetadata(null));

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
#if (SILVERLIGHT || WinRT)
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
            return (bool)GridUtil.GetValueInherited(dpo, HasFocusWithinProperty, false);
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

        private static readonly DependencyProperty IsElementSwappedProperty = DependencyProperty.RegisterAttached(
            "IsElementSwapped",
            typeof(bool),
            typeof(VirtualizingCellsControl),
            new PropertyMetadata(null));

        public static bool GetIsElementSwapped(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(VirtualizingCellsControl.IsElementSwappedProperty);
        }

        public static void SetIsElementSwapped(DependencyObject dpo, bool value)
        {
            dpo.SetValue(VirtualizingCellsControl.IsElementSwappedProperty, value);
        }
#else
        public static bool GetHasFocusWithin(UIElement el)
        {
            return el.IsKeyboardFocusWithin;
        }
#endif
        #endregion
        #region EnableRenderOptimization

        public bool EnableRenderOptimization
        {
            get { return (bool)GetValue(EnableRenderOptimizationProperty); }
            set { SetValue(EnableRenderOptimizationProperty, value); }
        }

        public static readonly DependencyProperty EnableRenderOptimizationProperty =
            DependencyProperty.Register("EnableRenderOptimization", typeof(bool), typeof(VirtualizingCellsControl), new PropertyMetadata(false));

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether [show grid lines].
        /// </summary>
        /// <value><c>true</c> if [show grid lines]; otherwise, <c>false</c>.</value>
        public bool ShowGridLines
        {
            get { return (bool)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(VirtualizingCellsControl), new PropertyMetadata(false, OnShowGridLinesChanged));

        private static void OnShowGridLinesChanged(DependencyObject obj,DependencyPropertyChangedEventArgs args)
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

#if (!SILVERLIGHT && !WinRT)
            renderedCells = new RenderedCellsManager(this.InnerFrame.Children);

            BackgroundFrame.Children.Add(cellBackgroundDrawingVisuals);
            BackgroundFrame.Children.Add(cellBackgroundRenderStyleDrawingVisuals);
            ForegroundFrame.Children.Add(foregroundFrameCellsVisual);
            ForegroundFrame.Children.Add(foregroundFrameBorderVisual);
#else
            BackgroundFrame.Children.Add(cellBackgrounds);
            BackgroundFrame.Children.Add(foregroundFrameBorders);
#endif
            VisualContainer.SetWantsMouseInput(this, true);

            //AddHandler(PreviewMouseMoveEvent, new MouseEventHandler(previewMouseMove));//, true);
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

        #region Keyboard and ScrollInDirection
#if !WinRT
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
        /// Implements handling for the KeyDown event. When
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
            bool isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.None)
            {
                bool isRightToLeft = false;// base.FlowDirection == FlowDirection.RightToLeft;
                switch (e.Key)
                {
                    case Key.PageUp:
                        PageUp();
                        e.Handled = true;
                        return;

                    case Key.PageDown:
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
#endif
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
                    if (ScrollColumns is PixelScrollAxis)
                        constraint.Width = Math.Min(constraint.Width, ((PixelScrollAxis)ScrollColumns).TotalExtent + 1);
                }
            }
            else
            {
                if (!IsDoubleValueSet(FrameworkElement.HeightProperty))
                {
                    // Grid is shown in ViewBox or another panel - does no scrolling by itself.
                    if (ScrollRows is PixelScrollAxis)
                        constraint.Height = ((PixelScrollAxis)ScrollRows).TotalExtent + 1;
                }

                if (!IsDoubleValueSet(FrameworkElement.WidthProperty))
                {
                    if (ScrollColumns is PixelScrollAxis)
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
            {
                if (coveredCellsLayout != null)
                    coveredCellsLayout.SetDirty();
                if (overlappingCellsLayout != null)
                    overlappingCellsLayout.SetDirty();
                //backgroundsDirty = true;
                //bordersDirty = true;
            }
            base.OnInvalidated(isArrangeDirty);
        }

        #region OnArrangeContent
        /// <summary>
        /// Arranges the cells row by row with the <see cref="ArrangeCellUIElements"/> method. For each cell
        /// the virtual <see cref="OnArrangeCell"/> method is called. OnArrangeCell gets
        /// the <see cref="ICellRenderer"/> for a cell and calls its <see cref="ICellRenderer.Arrange"/>
        /// method.<para/>
        /// ArrangeCellUIElements creates new UIElements for cells scrolled into view or unload UIElements for 
        /// cells scrolled out of view. <para/>
        /// OnArrangeContent also arranges covered cells, spanned backgrounds, 
        /// cell borders and combines cells with same cell background 
        /// (<see cref="ArrangeCombinedCellBackgrounds"/>) to reduce number 
        /// of drawing operations.
        /// </summary>
        /// <param name="arrangeSize"></param>
        protected override void OnArrangeContent(Size arrangeSize)
        {
            if (SuspendArrange)
                return;
            this.arrangeSize = arrangeSize;
            // Covered cells
            ArrangeCoveredCells(arrangeSize);

            //Image Cells
            ArrangeOverlappingCells(arrangeSize);

            // Draw one background across multiple cells
            ArrangeCellSpanBackgrounds(arrangeSize);

            // Create new UIElements for cells scrolled into view or unload UIElements for cells scrolled out of view.
            ArrangeCellUIElements(arrangeSize);

            ArrangedCellUIElements.RefreshDirtyCellUIElementsContent();

            // Calculate Borders - they will be rendered later in OnRender.
            ArrangeCellBorders(arrangeSize);

            // Calculate Cells Background - combine cells with same brush into one draw operation.
            ArrangeCombinedCellBackgrounds(arrangeSize);
#if (SILVERLIGHT || WinRT)

            RenderCellBackgounds(arrangeSize);

            RenderCellBorders();
            /* Commented it out, but left code here just in case issue with 
               chart in grid needs to be looked into again.*/
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    AutoMeasureRecursive(Content as Panel);
            //    AutoArrangeRecursive(Content as Panel);
            //}));

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
            //backgroundsDirty = false;
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

#if WPF
                            Brush background = GetCellBackground(ci, false);
#else
                            Brush background = GetCellBackground(ci);
#endif
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
                    if (!previousCellInfo.CanCombineCellBackground(nextci))
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

            //if (backgroundSpan.startRow - backgroundSpan.endRow < 0
            //    || backgroundSpan.startColumn - backgroundSpan.endColumn < 0)
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
#if WPF
        protected virtual Brush GetCellBackground(IRenderCellInfo ci, bool combineBackgrounds)
#else
        protected virtual Brush GetCellBackground(IRenderCellInfo ci)
#endif
        {
            // If background stored in CellInfo is not a brush and instead a description of a brush (e.g. GridBrushInfo)
            // this method is a good place to convert that to a brush and cache it for reuse in other cells.
            return ci.GetCellBackground() as Brush;
        }
        #endregion

        #region CoveredCells, OverlappingCell

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

            coveredCellsLayout.ArrangeCellSpans(arrangeSize, coveredCellsProvider.IsEmpty, true);
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

        //OverlappingCell
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
            if (overlappingCellsProvider == null)
                return;

            if (overlappingCellsLayout == null)
                overlappingCellsLayout = new VisibleCellSpanLayout<VisibleOverlappingCellInfo>(this, new GetCellSpanDelegate(GetOverlappingCell));

            overlappingCellsLayout.ArrangeCellSpans(arrangeSize, overlappingCellsProvider.IsEmpty, false);
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
        bool SuspendArrange = false;
        public virtual void UnloadArrangedCells()
        {
            SuspendArrange = true;
            ArrangedCellUIElements.UnloadAll();
#if (!SILVERLIGHT && !WinRT)
            RenderedCellVisuals.Invalidate();
#endif
            SuspendArrange = false;
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
#if (!SILVERLIGHT && !WinRT)
            RenderedCellVisuals.Invalidate(cellRowColumnIndex);
#endif
            InvalidateVisual(true);

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
            ArrangedCellUIElements.Invalidate(span);
#if (!SILVERLIGHT && !WinRT)            
            RenderedCellVisuals.Invalidate(span);
#endif
            InvalidateVisual(false);
        }

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
            if (combinedCellBackgroundIdsTable.Count > 0)
            {
                VisibleLineInfo visibleRow = ScrollRows.GetVisibleLineAtLineIndex(rowIndex);
                VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(columnIndex);
                if (visibleRow != null && visibleColumn != null)
                {
                    //backgroundsDirty = true;
                }
            }
        }

        #endregion

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
            try
            {
                ScrollRows.FreezeVisibleLines();
                foreach (VisibleLineInfo visibleLine in visibleRows)
                {
                    if (EnableRenderOptimization)
                    {
                        //If the size gets changed or horizontally scrolled then we need to arrange the all the rows
                        if (arrangedCellUIElements.unloadCellUIElements.Rows.ContainsKey(visibleLine.LineIndex) && !isScollColumnChanged && !isSizeChanged)
                        {
                            arrangedCellUIElements.aliveCellUIElements.Rows.Add(visibleLine.LineIndex, arrangedCellUIElements.unloadCellUIElements.Rows[visibleLine.LineIndex]);
                            ArrangeRowHelper(visibleLine);
                            arrangedCellUIElements.unloadCellUIElements.Rows.Remove(visibleLine.LineIndex);
                            continue;
                        }
                    }
                    ArrangeRow(visibleLine, corner);
                }

                ArrangedCellUIElements.ConcludeArrange();
                isScollColumnChanged = false;
                isSizeChanged = false;
            }
            finally
            {
                ScrollRows.UnfreezeVisibleLines();
            }
        }

        void ArrangeRowHelper(VisibleLineInfo visibleRow)
        {
            Rect cellRect = new Rect(0, visibleRow.Origin, ScrollColumns.ViewSize, visibleRow.Size);
            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();
            foreach (VisibleLineInfo visibleColumn in visibleColumns)
            {
                cellRect.X = visibleColumn.Origin;
                cellRect.Width = visibleColumn.Size;
                RowColumnIndex cellRowColumnIndex = new RowColumnIndex(visibleRow.LineIndex, visibleColumn.LineIndex);
                CellUIElements cellUIElements;
                ArrangeCellArgs aca = new ArrangeCellArgs(this, visibleRow, visibleColumn, cellRect, InternalGetCellInfo(visibleRow, visibleColumn));
                if (arrangedCellUIElements.unloadCellUIElements != null && arrangedCellUIElements.unloadCellUIElements.TryGetValue(cellRowColumnIndex, out cellUIElements))
                {
                    VisibleOverlappingCellInfo icSpan = GetVisibleOverlappingCell(visibleRow, visibleColumn);
                    //Floating Image cell
                    if (icSpan != null)
                    {
                        if (icSpan.arrangeId == visibleOverlappingCellsArrangeId)
                            continue;

                        if (icSpan.IsAmbiguousSection)
                            continue;

                        icSpan.arrangeId = visibleOverlappingCellsArrangeId;
                        aca.VisibleOverlappingCellInfo = icSpan;
                        aca.CellRect = icSpan.ExactBounds;

                        if (arrangedCellUIElements.aliveCellUIElements.ContainsKey(aca.CellRowColumnIndex))
                            arrangedCellUIElements.aliveCellUIElements.Remove(aca.CellRowColumnIndex);

                        aca.IsProcessingArrangeCellUIElements = true;
                        ArrangeCell(aca, ElementFrame);
                        continue;
                    }

                    VisibleCoveredCellInfo ccSpan = GetVisibleCoveredCell(visibleRow, visibleColumn);
                    //Covered Cell
                    if (ccSpan != null)
                    {
                        if (ccSpan.arrangeId == visibleCoveredCellsArrangeId)
                            continue;

                        if (ccSpan.IsAmbiguousSection)
                            continue;

                        if (hasReArranged && ccSpan.Top != visibleRow.LineIndex && ccSpan.Left != visibleColumn.LineIndex)
                            continue;

                        ccSpan.arrangeId = visibleCoveredCellsArrangeId;
                        aca.VisibleCoveredCellInfo = ccSpan;
                        aca.CellRect = ccSpan.ExactBounds;
                    }

                    //Overlapping Cell (Floating cells)
                    if (GetFloatCell(aca.CellRowColumnIndex))
                    {
                        if (arrangedCellUIElements.aliveCellUIElements.ContainsKey(aca.CellRowColumnIndex))
                            arrangedCellUIElements.aliveCellUIElements.Remove(aca.CellRowColumnIndex);
                        aca.IsProcessingArrangeCellUIElements = true;
                        ArrangeCell(aca, InnerFrame);
                        continue;
                    }

                    aca.CellRect = aca.SubtractBorderMargins(aca.CellRect, aca.CellInfo.GetBorderMargins());
                    ICellRenderer renderer = GetCellRenderer(aca.CellInfo);
                    if (renderer != null && cellUIElements.UIElements.Count > 0)
                    {
                        renderer.SetBounds(cellUIElements.UIElements[0], aca.CellRect, aca.ForceMeasure, false);
                    }
                }
                else
                {
                    VisibleOverlappingCellInfo icSpan = GetVisibleOverlappingCell(visibleRow, visibleColumn);
                    if (icSpan != null)
                    {
                        if (icSpan.arrangeId == visibleOverlappingCellsArrangeId)
                            continue;

                        if (icSpan.IsAmbiguousSection)
                            continue;

                        icSpan.arrangeId = visibleOverlappingCellsArrangeId;
#if!WinRT
                        aca.VisibleOverlappingCellInfo = icSpan;
#endif
                        aca.CellRect = icSpan.ExactBounds;

                        aca.IsProcessingArrangeCellUIElements = true;
                        ArrangeCell(aca, ElementFrame);
                        continue;
                    }

                    VisibleCoveredCellInfo ccSpan = GetVisibleCoveredCell(visibleRow, visibleColumn);
                    //Covered Cell
                    if (ccSpan != null)
                    {
                        if (ccSpan.arrangeId == visibleCoveredCellsArrangeId)
                            continue;

                        if (ccSpan.IsAmbiguousSection)
                            continue;

                        ccSpan.arrangeId = visibleCoveredCellsArrangeId;
                        aca.VisibleCoveredCellInfo = ccSpan;
                        aca.CellRect = ccSpan.ExactBounds;
                    }

                    aca.IsProcessingArrangeCellUIElements = true;
                    ArrangeCell(aca, InnerFrame);
                }
            }
        }

        bool isScollColumnChanged = false;
        protected override void OnScrollColumnsChanged()
        {
            if (!isScollColumnChanged)
                isScollColumnChanged = true;
        }

        bool isSizeChanged = false;
        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            if (!isSizeChanged)
                isSizeChanged = true;
        }

        protected virtual bool GetFloatCell(RowColumnIndex cell)
        {
            return false;
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
#if !WinRT
                VisibleOverlappingCellInfo icSpan = GetVisibleOverlappingCell(visibleRow, visibleColumn);
                if (icSpan != null)
                {
                    if (icSpan.arrangeId == visibleOverlappingCellsArrangeId)
                        continue;

                    if (icSpan.IsAmbiguousSection)
                        continue;
                    
                        icSpan.arrangeId = visibleOverlappingCellsArrangeId;
                        aca.VisibleOverlappingCellInfo = icSpan;
                        aca.CellRect = icSpan.ExactBounds;
                        //aca.VisibleImageCellInfo.forceClipping = true;
                        //aca.VisibleImageCellInfo.CellSpan.ClipRows = true;
                        //aca.VisibleImageCellInfo.CellSpan.ClipColumns = true;
                        //aca.IsProcessingArrangeCellUIElements = true;
                    
                    //if (icSpan.Left == visibleColumn.LineIndex && icSpan.Top == visibleRow.LineIndex)
                    //{
                        //aca.ShouldSwapUIElements = true;
                        ArrangeCell(aca, ElementFrame);
                        continue;
                    //}
                }
#endif
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
                ArrangeCell(aca, InnerFrame);
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

        void ArrangeCell(ArrangeCellArgs aca,Panel frame)
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
                    // In WPF, we simply removed and reinserted the UIElement in the
                    // new canvas, but this causes glitches with Silverlight. It is better
                    // to simple create a new UIElement, initialize it. The old element
                    // gets hidden and recycled in PreArrangeCell.
                    VirtualizingCellsControlChildFrame oldCanvas = VisualTreeHelper.GetParent(el) as VirtualizingCellsControlChildFrame;
                    if (oldCanvas == null)
                        canvas.Children.Add(el);
                }

                OnArrangeCell(aca);
            }
            finally
            {
                isInArrage = false;
            }

            ArrangedCellUIElements.PostArrangeCell(aca);

        }

        private VirtualizingCellsControlChildFrame GetChildFrame(CellArgs aca, Panel frame)
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

            if (aca.VisibleOverlappingCellInfo != null)
            {
                VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(aca.VisibleOverlappingCellInfo.CellSpan.Right);
                VisibleLineInfo visibleRow = ScrollRows.GetVisibleLineAtLineIndex(aca.VisibleOverlappingCellInfo.CellSpan.Bottom);
                CellSpanInfo span = aca.VisibleOverlappingCellInfo.CellSpan;

                if (aca.VisibleOverlappingCellInfo.spansMultipleHorizontalSections
                   || visibleColumn == null && (isRowHeaderAtLeftSide || isRowFooterAtRightSide))
                {
                    DoubleSpan xSpan = ScrollColumns.GetVisibleLinesClipPoints(span.Left, span.Right);
                    aca.VisibleOverlappingCellInfo.forceClipping |= true;
                    aca.VisibleOverlappingCellInfo.clippedBounds.Width = xSpan.Length;
                    isRowHeaderAtLeftSide = true;
                    isRowFooterAtRightSide = true;
                }

                if (aca.VisibleOverlappingCellInfo.spansMultipleVerticalSections
                    || visibleRow == null && (isColumnHeaderAtTop || isColumnFooterAtBottom))
                {
                    DoubleSpan ySpan = ScrollRows.GetVisibleLinesClipPoints(span.Top, span.Bottom);
                    aca.VisibleOverlappingCellInfo.forceClipping |= true;
                    aca.VisibleOverlappingCellInfo.clippedBounds.Height = ySpan.Length;
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
            {
                renderer.Arrange(aca);
            }
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
            {
                renderer.PrepareUIElements(aca, uiElements, canvas);
            }
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
            //bordersDirty = false;
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

        private bool CanCombineCellBorder(IRenderCellInfo previousCellInfo, IRenderCellInfo ci, Pen previousBorder, Pen border, CellBorderSide borderSide)
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
            //return ci.GetCellBorder(borderSide) as Pen;
            return ci.GetCellBorder(borderSide) as Pen;
        }

        #endregion

        #endregion

        #region Silverlight: Render CellBorders and Backgrounds
#if (SILVERLIGHT || WinRT)

        protected virtual  void RenderCellBorders()
        {
            if (!needRenderBorders)
            {
#if RectangleAsBorder
                foreach (UIElement el in cellBackgrounds.Children)
                    el.Arrange(GetRenderBounds(el));
#endif
                return;
            }

            needRenderBorders = false;
            foregroundFrameBorders.Children.Clear();

            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();

            // Grid Lines
            foreach (CellBorderRangeList innerBorders in verticalLines)
            {
                VisibleLineInfo visibleColumn = innerBorders.visibleLine;
                CellBorderSide borderSide = innerBorders.borderSide;

                foreach (CellBorderRange borderRange in innerBorders)
                {
                    VisibleLineInfo firstRow = visibleRows[borderRange.first];
                    VisibleLineInfo lastRow = visibleRows[borderRange.last];

                    Rect r = GridUtil.FromLTRB(Math.Floor(visibleColumn.ClippedOrigin), Math.Floor(firstRow.ClippedOrigin), Math.Floor(visibleColumn.ClippedCorner), Math.Floor(lastRow.ClippedCorner));

                    OnRenderBorderShape(r, r, borderSide, borderRange);
                }
            }

            foreach (CellBorderRangeList innerBorders in horizontalLines)
            {
                VisibleLineInfo visibleRow = innerBorders.visibleLine;
                CellBorderSide borderSide = innerBorders.borderSide;

                foreach (CellBorderRange borderRange in innerBorders)
                {
                    VisibleLineInfo firstColumn = visibleColumns[borderRange.first];
                    VisibleLineInfo lastColumn = visibleColumns[borderRange.last];

                    Rect r = GridUtil.FromLTRB(Math.Floor(firstColumn.ClippedOrigin), Math.Floor(visibleRow.ClippedOrigin), Math.Floor(lastColumn.ClippedCorner), Math.Floor(visibleRow.ClippedCorner));
                    
                    OnRenderBorderShape(r, r, borderSide, borderRange);
                }
            }
        }

        protected virtual void OnRenderBorderShape(Rect cellRect, Rect clipRect, CellBorderSide borderSide, CellBorderRange border)
        {
            
            double aliasingEffect = 0.5;
            Line line = new Line();
            cellRect = new Rect(Math.Round(clipRect.Left) + aliasingEffect, Math.Round(clipRect.Top) + aliasingEffect, Math.Round(clipRect.Width), Math.Round(clipRect.Height));
            if (border.pen == null)
            {
                line.Stroke = null;
            }
            else
            {
                line.Stroke = border.pen.Brush;
                line.StrokeThickness = border.pen.Thickness;
                switch (border.pen.Style)
                {
                    case BorderStyle.Dotted:
                        line.StrokeDashArray = new DoubleCollection() { 3.0, 3.0 };
                        break;
                    case BorderStyle.Dashed:
                        line.StrokeDashArray = new DoubleCollection() { 10.0, 10.0 };
                        break;
                    case BorderStyle.DashDot:
                        line.StrokeDashArray = new DoubleCollection() { 10.0, 3.0, 4.0, 3.0 };
                        break;
                    case BorderStyle.DashDotDot:
                        line.StrokeDashArray = new DoubleCollection() { 10.0, 3.0, 4.0, 3.0, 4.0, 3.0 };
                        break;
                    case BorderStyle.None:
                        if (ShowGridLines)
                        {
                            line.Stroke = new SolidColorBrush(Color.FromArgb(255, 218, 220, 221));
                            line.StrokeThickness = 0.50;
                        }
                        else
                        {
                            line.Opacity = 0.0d;
                            line.StrokeThickness = 0.00;
                        }
                        break;
                    case BorderStyle.Custom:
                        line.StrokeDashArray = border.pen.DashArray;
                        break;
                    case BorderStyle.Standard:
                    default:
                        break;
                }
            }
            double d = 0;//solidLine ? pen.Thickness : pen.Thickness / 2;
            switch (borderSide)
            {
                case CellBorderSide.Top:
                    line.X1 = cellRect.Left;
                    line.X2 = cellRect.Right;
                    line.Y1 = cellRect.Top;
                    line.Y2 = cellRect.Top + d;
                    break;
                case CellBorderSide.Bottom:
                    line.X1 = cellRect.Left;
                    line.X2 = cellRect.Right;
                    line.Y1 = cellRect.Bottom - d;
                    line.Y2 = cellRect.Bottom;
                    break;
                case CellBorderSide.Left:
                    line.X1 = cellRect.Left;
                    line.X2 = cellRect.Left + d;
                    line.Y1 = cellRect.Top;
                    line.Y2 = cellRect.Bottom;
                    break;
                case CellBorderSide.Right:
                    line.X1 = cellRect.Right - d;
                    line.X2 = cellRect.Right;
                    line.Y1 = cellRect.Top;
                    line.Y2 = cellRect.Bottom;
                    break;
            }

#if !RectangleAsBorder
            foregroundFrameBorders.Children.Add(line);
#else
            bool solidLine = true;

            Rect r = GridUtil.FromLTRB(line.X1, line.Y1, line.X2, line.Y2);
            Rectangle rectangleShape = new Rectangle();

            if (solidLine)
            {
                rectangleShape.Fill = pen.Brush;
            }
            else
            {
                // This is how to draw dotted lines.
                rectangleShape.Stroke = pen.Brush;
                rectangleShape.StrokeThickness = pen.Thickness / 2;
                DoubleCollection dc = new DoubleCollection();
                dc.Add(3);
                dc.Add(3);
                rectangleShape.StrokeDashArray = dc;
            }
            foregroundFrameBorders.Children.Add(rectangleShape);
            GridUtil.SetRenderBounds(rectangleShape, cellRect);
            rectangleShape.Arrange(r);
#endif
        }

        private void RenderCellBackgounds(Size arrangeSize)
        {
            if (!needRenderBackgrounds)
            {
                foreach (UIElement el in cellBackgrounds.Children)
                    el.Arrange(VisualContainer.GetRenderBounds(el));

                return;
            }

            needRenderBackgrounds = false;
            cellBackgrounds.Children.Clear();

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
                    Rect cellRect = GridUtil.FromLTRB(startColumn.Origin, startRow.Origin, endColumn.Corner, endRow.Corner);
                    Rect clipRect = GridUtil.FromLTRB(startColumn.ClippedOrigin, startRow.ClippedOrigin, endColumn.ClippedCorner, endRow.ClippedCorner);

                    cellRect.Width = cellRect.Width + 1;
                    cellRect.Height = cellRect.Height + 1;
                    clipRect.Height = clipRect.Height + 1;
                    clipRect.Width = clipRect.Width + 1;

                    Rectangle rectangleShape = new Rectangle();
                    rectangleShape.Fill = s.background;
                    rectangleShape.Height = cellRect.Height;
                    rectangleShape.Width = cellRect.Width;
                    cellBackgrounds.Children.Add(rectangleShape);
                    if (cellRect != clipRect)
                    {
                        Rect c = clipRect;
                        c.X -= cellRect.X;
                        c.Y -= cellRect.Y;
                        GridUtil.SetClipRect(rectangleShape, c);
                    }
                    else
                        rectangleShape.Clip = null;
                    VisualContainer.SetRenderBounds(rectangleShape, cellRect);
                    rectangleShape.Arrange(cellRect);
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

                        Rectangle rectangleShape = new Rectangle();
                        rectangleShape.Fill = bg.CellSpanBackground.Background;
                        rectangleShape.Stroke = bg.CellSpanBackground.Border.Brush;
                        rectangleShape.StrokeThickness = bg.CellSpanBackground.Border.Thickness;
                        rectangleShape.Height = cellRect.Height;
                        rectangleShape.Width = cellRect.Width;
                        cellBackgrounds.Children.Add(rectangleShape);
                        if (bg.ClippedBounds != cellRect)
                        {
                            Rect c = GetClipRect((ScrollAxisRegion)bg.RowSection, (ScrollAxisRegion)bg.ColumnSection);
                            c.X -= cellRect.X;
                            c.Y -= cellRect.Y;
                            GridUtil.SetClipRect(rectangleShape, c);
                        }
                        else
                            rectangleShape.Clip = null;
                        VisualContainer.SetRenderBounds(rectangleShape, cellRect);
                        rectangleShape.Arrange(cellRect);
                    }
                }
            }
        }
#endif
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
            }
            visuals = GetCellUIElements(cellRowColumnIndex);

            IRenderCellInfo ci = GetRenderCellInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
            ArrangeCellArgs aca = new ArrangeCellArgs(this, visibleRow, visibleColumn, cellRect, GetRenderCellInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex));
            aca.VisibleCoveredCellInfo = ccSpan;
#if!WinRT
            aca.VisibleOverlappingCellInfo = icSpan;
#endif
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
            VirtualizingCellsControlChildFrame canvas = GetChildFrame(aca, InnerFrame);
            ArrangeCellHelper(aca, canvas);
            //if (aca.HasVisuals)
            //    EraseRenderedCell(aca.CellRowColumnIndex, aca.OriginalCellRect);
            return true;
        }

#if (SILVERLIGHT || WinRT)
        public bool DelayedSwapCellUIElements(RowColumnIndex cellRowColumnIndex, bool swapToActualCellRenderer)
        {
            var aca = this.PrepareCreateOrRefreshCellUIElements(cellRowColumnIndex);
            if (aca == null || !aca.VisibleColumn.IsVisible || !aca.VisibleRow.IsVisible || aca.CellUIElements.UIElements.Count <= 0)
                return false;

            var uiElement = aca.CellUIElements.UIElements[0];
            var canvas = this.GetChildFrame(aca,InnerFrame);
            // remove from canvas
            canvas.Children.Remove(uiElement);
            aca.CellUIElements.UIElements.Clear();
            // remove from arrange cell manager
            this.arrangedCellUIElements.ClearAliveCell(cellRowColumnIndex);
            if (!swapToActualCellRenderer)
            {
                VirtualizingCellsControl.SetIsElementSwapped(uiElement, false);
            }
            aca.ShouldCreateVisuals = true;
            aca.ShouldSwapUIElements = swapToActualCellRenderer;
            this.ArrangeCell(aca, InnerFrame);
            if (swapToActualCellRenderer)
            {
                var swappedUIElement = aca.CellUIElements.UIElements[0];
                VirtualizingCellsControl.SetIsElementSwapped(swappedUIElement, true);
            }

            /*Dispatcher.BeginInvoke(new Action(() =>
            {
                AutoMeasureRecursive(Content as Panel);
                AutoArrangeRecursive(Content as Panel);
            }));*/

            return true;
        }
#endif

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

            aca.ShouldReinitializeContent = true;
            if (aca.VisibleOverlappingCellInfo == null)
            {
                VirtualizingCellsControlChildFrame canvas = GetChildFrame(aca, InnerFrame);
                ArrangeCellHelper(aca, canvas);
            }
        }

        #endregion

        #region PointToCellRowColumnIndex
#if !WinRT
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
#endif

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

                //ImageCellInfo ici = GetImageCell(visibleRow,visibleColumn);
                //if (ici != null)
                //    return new RowColumnIndex(ici.Top, ici.Left);
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

        private IRenderCellInfo InternalGetCellInfo(VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn)
        {
            if (coveredCellsProvider != null && !coveredCellsProvider.IsEmpty)
            {
                CoveredCellInfo coveredCell = GetCoveredCell(visibleRow, visibleColumn);
                if (coveredCell != null)
                    return InternalGetCellInfo(coveredCell);
            }

            return GetRenderCellInfo(visibleRow.LineIndex, visibleColumn.LineIndex);
        }

        private IRenderCellInfo InternalGetCellInfo(CoveredCellInfo coveredCellInfo)
        {
            return GetRenderCellInfo(coveredCellInfo.Top, coveredCellInfo.Left);
        }

        #endregion

        #region Private CellBorderRangeList, CellBorderRange classes
        class CellBorderRangeList : List<CellBorderRange>,IDisposable
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
                foreach (var item in this)
                {
                    item.pen = null;
                }
                this.Clear();
                this.visibleLine = null;
            }
        }

        public class CellBorderRange
        {
            public CellBorderRange(Pen pen, int first)
            {
                this.pen = pen;
                this.first = first;
                this.last = first;
            }

            internal Pen pen;
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
                    ArrangeCellSpans(this._cellsControl.RenderSize, false, true);
            }

            public void ArrangeCellSpans(Size arrangeSize, bool isEmpty, bool spansMultipleSections)
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
                CellSpanInfo perCellSpanInfo = null;
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
                                VisibleLineInfo visibleColumn = visibleColumns[visibleColumnIndex];
                                CellSpanInfo cellSpanBackgroundInfo = GetCellSpan(visibleRow.LineIndex, visibleColumn.LineIndex);

                                T foundSpan = visibleCellSpansTable[visibleRowIndex][visibleColumnIndex];
                                // Skip this if cell belongs to an CellSpan from a previous row.
                                if (spansMultipleSections)
                                {
                                    if (foundSpan != null)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (foundSpan != null && cellSpanBackgroundInfo != null && foundSpan.CellSpan == cellSpanBackgroundInfo)
                                    {
                                        // Skip if cell belongs to an overlapping cell.
                                        continue;
                                    }
                                }

                                if (cellSpanBackgroundInfo != null && perCellSpanInfo == cellSpanBackgroundInfo)
                                {
                                    //visibleColumnIndex = cellSpanBackgroundInfo.Right;
                                    continue;
                                }
                            
                                if (cellSpanBackgroundInfo != null)
                                {
                                    bool isAmbiguous = false;
                                    perCellSpanInfo = cellSpanBackgroundInfo;
                                    if (spansMultipleSections)
                                    {
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


        protected override void dispose()
        {
            if (arrangedCellUIElements != null)
            {
                if (this.arrangedCellUIElements.aliveCellUIElements != null)
                    this.arrangedCellUIElements.aliveCellUIElements.RemoveAll();
                if (this.arrangedCellUIElements.unloadCellUIElements != null)
                    this.arrangedCellUIElements.unloadCellUIElements.RemoveAll();
                this.arrangedCellUIElements = null;
            }
            this.cellBackgrounds.Children.Clear();
            this.cellBackgrounds = null;
            this.foregroundFrameBorders.Children.Clear();
            this.foregroundFrameBorders = null;
            this.combinedCellBackgroundIdsTable.Clear();
            this.combinedCellBackgroundIdsTable = null;
            this.combinedCellBackgroundsList.Clear();
            this.combinedCellBackgroundsList = null;
            this.cellSpanBackgroundsProvider = null;
            this.ColumnWidthsProvider = null;
            this.RowHeightsProvider = null;
            if (coveredCellsLayout != null)
            {
                this.coveredCellsLayout.ScrollColumns.Dispose();
                this.coveredCellsLayout.ScrollRows.Dispose();
                this.coveredCellsLayout = null;
            }
            this.coveredCellsProvider = null;
            if (framesListWithBackgoundSpans != null)
                this.framesListWithBackgoundSpans.Clear();
            if (this.overlappingCellsLayout != null)
            {
                this.overlappingCellsLayout.ScrollColumns.Dispose();
                this.overlappingCellsLayout.ScrollRows.Dispose();
                this.overlappingCellsLayout = null;
            }
            this.overlappingCellsProvider = null;
            foreach (var item in horizontalLines)
            {
                item.Dispose();
            }
            this.horizontalLines.Clear();
            this.horizontalLines = null;
            foreach (var item in verticalLines)
            {
                item.Dispose();
            }
            this.verticalLines.Clear();
            this.verticalLines = null;
            base.dispose();
        }
    }
}