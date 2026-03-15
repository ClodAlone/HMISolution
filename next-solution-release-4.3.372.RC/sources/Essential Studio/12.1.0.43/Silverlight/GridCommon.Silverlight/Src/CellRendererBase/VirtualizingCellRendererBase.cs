#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

#if !WinRT
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
using System.Windows;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{
    // B = Base Class 
    using B = CellRendererBase<IRenderCellInfo>;
    // GridVirtualizingCellRendererBase and TreeVirtualizingCellRendererBase
    // share the same code. This would be an ideal candidate for a Template 
    // but it is not possible to specify the base class as Template argument. 
    // 
    // When making changes in one class they should be copy/pasted to the other class.

    // S = Style
    using S = IRenderCellInfo;

    #region Shared Code

    /// <summary>
    /// VirtualizingCellRendererBase is an abstract base class for cell renderers
    /// that need live UIElement visuals displayed in a cell. You can derive from
    /// this class and provide the type of the UIElement you want to show inside cells
    /// as type parameter. The class provides strong typed virtual methods for 
    /// initializing content of the cell and arranging the cell visuals.
    /// <para/>
    /// The class manages the creation 
    /// of cells UIElement objects when the cell is scrolled into view and also 
    /// unloading of the elements. The class offers an optimization in which 
    /// elements can be recycled when <see cref="AllowRecycle"/> is set. 
    /// In this case when a cell is scrolled out of view
    /// it is moved into a recycle bin and the next time a new element is scrolled into
    /// view the element is recovered from the recycle bin and reinitialized with the
    /// new content of the cell.<para/>
    /// Another optimization is support for cells rendering themselves directly to the
    /// drawing context. When <see cref="SupportsRenderOptimization"/> is true the 
    /// UIElement will only be created
    /// when the user moves the mouse over the cell or if the UIElement is needed for
    /// other reasons.<para/>
    /// After a UIElement was created the virtual methods <see cref="WireUIElement"/> 
    /// and <see cref="UnwireUIElement"/> are called to wire any event listeners.
    /// <para/>
    /// Updates to appearance and content of child elements, creation and unloading
    /// of elements will not trigger ArrangeOverride or Render calls in parent canvas.
    /// <para/>
    /// In Grid and Tree control you should not derive from this class. Instead you should
    /// derive from the GridVirtualizingCellRendererBase and TreeVirtualizingCellRendererBase classes.
    /// These classes are the exact same code base as this class with the only difference that they 
    /// derive from GridCellRendererBase and TreeCellRenderer base classes instead. It is 
    /// currently not possible with C# to the base class as template type parameter. This is
    /// the reason for this copy/paste approach for the codebase of this class.
    /// </summary>
    /// <typeparam name="T">The type of the UIElement that should be placed inside cells</typeparam>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public abstract class VirtualizingCellRendererBase<T> : B, IDisposable
          where T : FrameworkElement, new()   // FrameworkElement required for Unloaded event.
    {
        VirtualizingCellUIElementBin<T> recycleBin = new VirtualizingCellUIElementBin<T>();
        bool supportsRenderOptimization = false;
        bool allowTransparentBackground = true;
        bool allowRecycle = false;
#if (SILVERLIGHT || WinRT)
        static Brush transparentBrush = new SolidColorBrush(Colors.Transparent);
#endif

        bool allowRenderTransform = false; // this is working only when the number of live elements is limited. Once
        // there are more live UIElements then setting .RenderTransform does have no effect. I keep this false
        // and have code ready for future use.

        /// <summary>
        /// Gets or sets a value indicating whether elements can be recycled when scrolled out of view.
        /// In this case when a cell is scrolled out of view
        /// it is moved into a recycle bin and the next time a new element is scrolled into
        /// view the element is recovered from the recycle bin and reinitialized with the
        /// new content of the cell. The default value is false.
        /// </summary>
        /// <value><c>true</c> if elements can be recycled when scrolled out of view; otherwise, <c>false</c>.</value>
        public bool AllowRecycle
        {
            get { return allowRecycle; }
            set { allowRecycle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the background of the UIElement visual
        /// placed in the cell can be set to <see cref="Brushes.Transparent"/>. This is needed
        /// in order for the cells control to draw the cell background color behind the UIElement.
        /// The default value is true.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow transparent background]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowTransparentBackground
        {
            get { return allowTransparentBackground; }
            set { allowTransparentBackground = value; }
        }

        /// <summary>
        /// Gets or sets whether the renderer supports rendering itsself directly to the
        /// drawing context. When this is possible the UIElement will only be created
        /// when the user moves the mouse over the cell or if the UIElement is needed for
        /// other reasons, e.g. animate after change. The benefit of rendering directly to the 
        /// DrawingContext instead of creating the UIElement is a much improved scrolling 
        /// performance. The default value is false.
        /// </summary>
        public new bool SupportsRenderOptimization
        {
            get { return supportsRenderOptimization; }
            set { supportsRenderOptimization = value; }
        }

        /// <summary>
        /// Called from <see cref="ICellRenderer.Arrange"/> to
        /// arrange the cells UIElement children. The method checks
        /// <see cref="ArrangeCellArgs.ShouldCreateVisuals"/> and if this is true
        /// it will initialize the newly created UIElement with attached properties
        /// about CellRowColumnIndex, RenderCellInfo, CellRenderer, 
        /// initialize its default properties with a call to <see cref="InitializeDefaultProperties"/>
        /// and its cell specific content with a call to <see cref="InitializeContent"/>.
        /// The UIElement is arranged on the canvas with a call to <see cref="ArrangeUIElement"/>.
        /// </summary>
        /// <param name="aca">The arange cell layout information.</param>
        /// <param name="style">The cell style info.</param>
        protected override void OnArrange(ArrangeCellArgs aca, S style)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("GridVirtualizingCellRenderer.OnArrange")) 
            {
#endif
            T uiElement = GetUIElement(aca.CellUIElements);
            if (uiElement != null)
            {
                if (aca.ShouldCreateVisuals || aca.ShouldReinitializeContent)
                {

                    VirtualizingCellsControl.SetCellRowColumnIndex(uiElement, new RowColumnIndex(aca.RowIndex, aca.ColumnIndex));
                    VirtualizingCellsControl.SetCellRenderer(uiElement, this);
                    VirtualizingCellsControl.SetCellsControl(uiElement, aca.CellsControl);
                    VirtualizingCellsControl.SetCellUIElement(uiElement, uiElement);

                    uiElement.Clip = null;
                    InitializeDefaultProperties(uiElement);
                    InitializeContent(uiElement, (S)aca.CellInfo);
                }

                VirtualizingCellsControl.SetArrangeCellArgs(uiElement, aca);

                ArrangeUIElement(aca, uiElement, style);
            }
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Set Backgrounds, BorderThickness, Padding and IsEnabled properties.
        /// </summary>
        /// <param name="uiElement"></param>
        protected virtual void InitializeDefaultProperties(T uiElement)
        {
#if (SILVERLIGHT || WinRT)
            Control c = uiElement as Control;
            if (c == null)
                return;
            if (AllowTransparentBackground)
                c.Background = transparentBrush;
            c.Padding = new Thickness(0);
            c.IsEnabled = true;
#else
            if (AllowTransparentBackground)
                uiElement.SetValue(Control.BackgroundProperty, Brushes.Transparent);
            uiElement.SetValue(Control.PaddingProperty, new Thickness(0));
            uiElement.SetValue(FrameworkElement.IsEnabledProperty, true);
#endif
        }


        /// <summary>
        /// Arranges the UI element by setting the Left, Top, Right, Bottom attached
        /// properties and its Width, Height, MaxWidth and MaxHeight properties. The
        /// visibility is set to Visibility.Visible and the elements Measure and
        /// Arrange methods are called.
        /// </summary>
        /// <param name="aca">The arrange cell layout.</param>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The cell style.</param>
        protected virtual void ArrangeUIElement(ArrangeCellArgs aca, T uiElement, S style)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("GridVirtualizingCellRenderer.ArrangeUIElement")) 
#endif
            SetBounds(uiElement, aca.CellRect, aca.ForceMeasure, false);
        }

        public static Rect GetBounds(UIElement el)
        {
            return VisualContainer.GetRenderBounds(el);
        }

        /// <summary>
        /// Helper method which arranges an UI element by setting the Left, Top, Right, Bottom attached
        /// properties and its Width, Height, MaxWidth and MaxHeight properties. The
        /// visibility is set to Visibility.Visible and the elements Measure and
        /// Arrange methods are called.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="rect">The rectangle.</param>
        protected void SetBounds(UIElement el, Rect rect)
        {
            SetBounds(el, rect, true, true);
        }

        /// <summary>
        /// Helper method which arranges an UI element by setting the Left, Top, Right, Bottom attached
        /// properties and its Width, Height, MaxWidth and MaxHeight properties. The
        /// visibility is set to Visibility.Visible and the elements Measure and
        /// Arrange methods are called.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="rect">The rectangle.</param>
        /// <param name="forceMeasure"></param>
        /// <param name="forceArrange"></param>
        protected override void SetBounds(UIElement el, Rect rect, bool forceMeasure, bool forceArrange)
        {
            if (!forceMeasure)
            {
                Rect elRect = GetBounds(el);
                Visibility visibility = rect.Height > 0 || rect.Width > 0 ? Visibility.Visible : Visibility.Collapsed;
                if (el == null || elRect.IsEmpty || GridUtil.GetSize(elRect) != GridUtil.GetSize(rect))
                    forceMeasure = true;
                // not calling arrange causes issues with nested grids. therefore proceed.
                else if (!forceArrange && !VirtualizingCellsControl.GetHasFocusWithin(el))
                {
                    if (rect == elRect)
                    {
                        ApplyTransformAndVisibility(el, visibility, 0, 0);
                        return;
                    }

                    if (allowRenderTransform)
                    {
                        ApplyTransformAndVisibility(el, visibility, rect.X - elRect.X, rect.Y - elRect.Y);
                        return;
                    }
                }
            }

            VisualContainer.SetRenderBounds(el, rect);

            if (!rect.IsEmpty)
            {
                el.SetValue(FrameworkElement.WidthProperty, rect.Width);
                el.SetValue(FrameworkElement.HeightProperty, rect.Height);
                el.SetValue(FrameworkElement.MaxWidthProperty, rect.Width);
                el.SetValue(FrameworkElement.MaxHeightProperty, rect.Height);
                Visibility visibility = rect.Height > 0 || rect.Width > 0 ? Visibility.Visible : Visibility.Collapsed;
                ApplyTransformAndVisibility(el, visibility, 0, 0);
                if (forceMeasure)
                {
                    // When a cells UIElement is recycled I sometimes ran into the issue
                    // that the element would not display correctly. Take the ComboBoxCellRenderer
                    // for example with ComboBox.IsEditable = false. It should display the
                    // display text for the selected item in a TextBlock. When the combobox
                    // was recycled the element would sometimes come up empty. I traced
                    // down the problem that the ApplyTemplate method (and thus the textblock)
                    // was only called at a later time when the parent was rendered.
                    //
                    // With debugging through .NET code I noticed that the ApplyTemplate was 
                    // called from MeasureCore and I can force this with a call to InvalidateMeasure.
                    // InvalidateMeasure however does not affect the MeasureValid of child elements.
                    // I need to recurse through all elements and call InvalidateMeasure for all
                    // of them.
                    // 
                    InvalidateMeasureRecursive(el);
                    el.Measure(GridUtil.GetSize(rect));
                    OnElementMeasured(el, GridUtil.GetSize(rect));
                }
            }
            else
            {
                ApplyTransformAndVisibility(el, Visibility.Collapsed, 0, 0);
                rect = new Rect(0, 0, 0, 0);
            }

            el.Arrange(rect);
            OnElementArranged(el, rect);
        }

        void InvalidateMeasureRecursive(UIElement obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                UIElement child = VisualTreeHelper.GetChild(obj, i) as UIElement;
                if (child != null)
                {
                    child.InvalidateMeasure();
#if (!SILVERLIGHT && !WinRT)
                    if (!child.IsMeasureValid)
                        InvalidateMeasureRecursive(child);
#endif
                }
            }
        }

        protected virtual void OnElementArranged(UIElement el, Rect rect)
        {
        }

        protected virtual void OnElementMeasured(UIElement el, Size size)
        {
        }

        private static void ApplyTransformAndVisibility(UIElement el, Visibility visibility, double offsetX, double offsetY)
        {
            if (visibility != el.Visibility)
                el.Visibility = visibility;

            TranslateTransform translate = null;
            if (offsetX != 0 || offsetY != 0)
            {
                translate = new TranslateTransform();
                translate.X = offsetX;
                translate.Y = offsetY;
            }
            TranslateTransform translate2 = el.RenderTransform as TranslateTransform;
            if (translate2 == null)
            {
                if (translate != null)
                {
#if (!SILVERLIGHT && !WinRT)
                    translate.Freeze();
#endif
                    el.RenderTransform = translate;
                }
            }
            else
            {
                if (translate != null)
                {
                    if (translate2.X != translate.X || translate2.Y != translate.Y)
                    {
#if (!SILVERLIGHT && !WinRT)
                        translate.Freeze();
#endif
                        el.RenderTransform = translate;
                    }
                }
                else
                {
#if (SILVERLIGHT || WinRT)
                    el.RenderTransform = null;
#else
                    el.RenderTransform = Transform.Identity;
#endif
                }
            }
        }


        /// <summary>
        /// Gets the UI element or null if the cell visuals do not have any UIElement visuals.
        /// </summary>
        /// <param name="cellUIElements">The cell visuals.</param>
        /// <returns></returns>
        public T GetUIElement(CellUIElements cellUIElements)
        {
            if (cellUIElements.UIElements.Count == 0)
                return null;
            return cellUIElements.UIElements[0] as T;
        }

        bool inInitializeContent = false;

        /// <summary>
        /// Gets a value indicating whether InitializeContent was called.
        /// </summary>
        /// <value><c>true</c> if InitializeContent was called; otherwise, <c>false</c>.</value>
        public bool InInitializeContent
        {
            get { return inInitializeContent; }
        }

        /// <summary>
        /// Called from <see cref="OnArrange"/> to initialize the content of the cell 
        /// using the information from the cell style (value, text,
        /// behavior etc.). The method calls the virtual <see cref="OnInitializeContent"/> which 
        /// you should override in your derived class.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="cellInfo">The cell style info.</param>
        public void InitializeContent(T uiElement, S cellInfo)
        {
            try
            {
                inInitializeContent = true;
                VirtualizingCellsControl.SetRenderCellInfo(uiElement, cellInfo);
                OnInitializeContent(uiElement, cellInfo);
            }
            finally
            {
                inInitializeContent = false;
            }
        }


        /// <summary>
        /// Called to initialize the content of the cell 
        /// using the information from the cell style (value, text,
        /// behavior etc.). You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="cellInfo">The cell style info.</param>
        public abstract void OnInitializeContent(T uiElement, S cellInfo);

        /// <summary>
        /// Called from <see cref="ICellRenderer.Arrange"/> to
        /// prepare the cells UIElement children.
        /// VirtualizingCellRendererBase overrides this method and
        /// creates new UIElements and wires them with the parent cells control.
        /// </summary>
        /// <param name="aca">The arange cell layout information.</param>
        /// <param name="uiElements">The UI elements.</param>
        /// <param name="canvas">The canvas to which any UIElement elements should be added.</param>
        /// <param name="cellInfo">The cell style info.</param>
        protected override void OnPrepareUIElements(ArrangeCellArgs aca, List<UIElement> uiElements, ScrollControlChildFrame canvas, S cellInfo)
        {
            if (SupportsRenderOptimization && aca.IsProcessingArrangeCellUIElements)
                return;
#if MEASURETIME
            //using (MeasureTime.Measure("GridVirtualizingCellRenderer.OnPrepareUIElements")) 
            {
#endif

            T uiElement = CreateOrRecycleUIElement(aca, canvas, cellInfo);

            // Wire events
            WireUIElement(uiElement);

            // Also save reference 
            uiElements.Add(uiElement);
#if MEASURETIME
            }
#endif
        }

        T CreateOrRecycleUIElement(ArrangeCellArgs aca, ScrollControlChildFrame canvas, S cellInfo)
        {
            T uiElement;

            if (AllowRecycle)
            {
                uiElement = recycleBin.Dequeue(canvas);

                if (uiElement != null)
                {
                    //uiElement.InvalidateMeasure();
                    //uiElement.InvalidateVisual();
                    return uiElement;
                }
            }

            uiElement = CreateUIElement(aca, cellInfo);

            // Add control to canvas frame
            //canvas.Children.Add(uiElement);	   // VirtualizingCellsControl.ArrangeCell will call children.Add.

            return uiElement;
        }


        /// <summary>
        /// Creates a new UIElement of type specified with the class type parameter.
        /// </summary>
        /// <returns></returns>
        protected virtual T CreateUIElement(ArrangeCellArgs aca, S cellInfo)
        {
            return new T();
        }

        /// <summary>
        /// Called from <see cref="ICellRenderer.UnloadUIElements"/> after a cell is scrolled out of view.
        /// VirtualizingCellRendererBase overrides this method and
        /// creates either removes the cell renderer visuals from the parent canvas
        /// or hide them and reuse it later in same canvas depending on whether
        /// <see cref="AllowRecycle"/> was set.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="visuals">The visuals.</param>
        protected sealed override void OnUnloadUIElements(VirtualizingCellsControl host, RowColumnIndex cellRowColumnIndex, CellUIElements visuals)
        {
#if MEASURETIME
            using (MeasureTime.Measure("GridVirtualizingCellRenderer.OnUnloadUIElements")) {
#endif
            bool isloaded = true;
#if (!SILVERLIGHT && !WinRT)
            isloaded = host.IsLoaded;
#endif
            if (!AllowRecycle || !isloaded)
                visuals.Unload(host); // calls parent.Children.Remove(uiElement), generates Unload event which calls UnwireUIElement
            else
            {
                T uiElement = GetUIElement(visuals);
                if (uiElement != null)
                {
                    uiElement.ClearValue(VisualContainer.CellRenderBoundsProperty);
                    if (!AllowRecycleIfIsKeyboardFocusWithin && VirtualizingCellsControl.GetHasFocusWithin(uiElement)) // avoid later trouble because element has focus.
                        visuals.Unload(host);
                    else
                    {
                        if (uiElement.RenderTransform != null)
                            uiElement.RenderTransform = null;
                        recycleBin.Enqueue(uiElement);
                        UnwireUIElement(uiElement);
                        visuals.UIElements.Clear();
#if (!SILVERLIGHT && !WinRT)
                        ScrollControlChildFrame oldCanvas = VisualTreeHelper.GetParent(uiElement) as ScrollControlChildFrame;
                        oldCanvas.Children.Remove(uiElement);
#endif
                    }
                }
            }
#if MEASURETIME
            }
#endif
        }

        bool allowRecycleIfIsKeyboardFocusWithin = false;

        public bool AllowRecycleIfIsKeyboardFocusWithin
        {
            get { return allowRecycleIfIsKeyboardFocusWithin; }
            set { allowRecycleIfIsKeyboardFocusWithin = value; }
        }

#if (SILVERLIGHT || WinRT)
        private void WireUIElement(T uiElement)
        {
            uiElement.GotFocus += new RoutedEventHandler(uiElement_GotFocus);
            uiElement.LostFocus += new RoutedEventHandler(uiElement_LostFocus);
            OnWireUIElement(uiElement);
        }

        private void UnwireUIElement(T uiElement)
        {
            uiElement.GotFocus -= new RoutedEventHandler(uiElement_GotFocus);
            uiElement.LostFocus -= new RoutedEventHandler(uiElement_LostFocus);
            OnUnwireUIElement(uiElement);
        }

        void uiElement_LostFocus(object sender, RoutedEventArgs e)
        {
            VirtualizingCellsControl.SetHasFocusWithin((T)sender, false);
        }

        void uiElement_GotFocus(object sender, RoutedEventArgs e)
        {
            VirtualizingCellsControl.SetHasFocusWithin((T)sender, true);
        }
#else
        private void WireUIElement(T uiElement)
        {
            uiElement.Unloaded += new RoutedEventHandler(uiElement_Unloaded);
            OnWireUIElement(uiElement);
        }

        private void UnwireUIElement(T uiElement)
        {
            uiElement.Unloaded -= new RoutedEventHandler(uiElement_Unloaded);
            OnUnwireUIElement(uiElement);
        }

        void uiElement_Unloaded(object sender, RoutedEventArgs e)
        {
            T uiElement = (T)sender;
            VirtualizingCellsControl cellsControl = VirtualizingCellsControl.GetCellsControl(uiElement);
            if (cellsControl == null || !cellsControl.ArrangedCellUIElements.Contains(VirtualizingCellsControl.GetCellRowColumnIndex(uiElement)))
                UnwireUIElement(uiElement);
        }
#endif

        /// <summary>
        /// Wire events from uiElement
        /// </summary>
        /// <param name="uiElement"></param>
        protected virtual void OnWireUIElement(T uiElement)
        {
        }

        /// <summary>
        /// Unwire previously wired events from uiElement. 
        /// </summary>
        /// <param name="uiElement"></param>
        protected virtual void OnUnwireUIElement(T uiElement)
        {
        }

        #region IDisposable Members

        /// <summary>
        /// This gets called when parent grid is unloaded. Unload all elements
        /// that were saved for later reuse.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                recycleBin.Unload();
            }
        }

        #endregion
    }
    #endregion
}
