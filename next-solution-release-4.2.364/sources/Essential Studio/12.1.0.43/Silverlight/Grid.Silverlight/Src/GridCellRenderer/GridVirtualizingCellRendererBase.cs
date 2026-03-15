#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows;
#if !WinRT
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Data;
using System.Threading.Tasks;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{

    // B = Base Class 
    using B = GridCellRendererBase;
    // GridVirtualizingCellRendererBase and TreeVirtualizingCellRendererBase
    // share the same code. This would be an ideal candidate for a Template 
    // but it is not possible to specify the base class as Template argument. 
    // 
    // When making changes in one class they should be copy/pasted to the other class.

    // S = Style
    using S = GridRenderStyleInfo;
    using System.Diagnostics;

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
    public abstract class GridVirtualizingCellRendererBase<T> : B, IDisposable
          where T : FrameworkElement, new()   // FrameworkElement required for Unloaded event.
    {
        VirtualizingCellUIElementBin<T> recycleBin = new VirtualizingCellUIElementBin<T>();
        VirtualizingCellUIElementBin<UIElement> recycleDefaultRendererBin = new VirtualizingCellUIElementBin<UIElement>();

#if !SILVERLIGHT
        bool supportsRenderOptimization = false;
#endif
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

#if WPF
        /// <summary>
        /// Gets or sets whether the renderer supports rendering itsself directly to the
        /// drawing context. When this is possible the UIElement will only be created
        /// when the user moves the mouse over the cell or if the UIElement is needed for
        /// other reasons, e.g. animate after change. The benefit of rendering directly to the 
        /// DrawingContext instead of creating the UIElement is a much improved scrolling 
        /// performance. The default value is false.
        /// </summary>
        public bool SupportsRenderOptimization
        {
            get { return supportsRenderOptimization; }
            set { supportsRenderOptimization = value; }
        }
#endif

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
            bool canProcessUIElement = true;
            T uiElement = uiElement = this.GetUIElement(aca.CellUIElements);
            if (this.SupportsRenderOptimization && uiElement != null)
            {
                canProcessUIElement = this.SupportsRenderOptimization && (aca.ShouldSwapUIElements || VirtualizingCellsControl.GetIsElementSwapped(uiElement));
            }
            if (canProcessUIElement && uiElement != null)
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
            else
            {
                var rendererElement = this.GetRendererElement(aca.CellUIElements);
                if (rendererElement != null)
                {
                    if (aca.ShouldCreateVisuals || aca.ShouldReinitializeContent)
                    {
                        VirtualizingCellsControl.SetCellRowColumnIndex(rendererElement, new RowColumnIndex(aca.RowIndex, aca.ColumnIndex));
                        VirtualizingCellsControl.SetCellRenderer(rendererElement, this);
                        VirtualizingCellsControl.SetCellsControl(rendererElement, aca.CellsControl);
                        VirtualizingCellsControl.SetCellUIElement(rendererElement, rendererElement);
                        rendererElement.Clip = null;
                        rectangle = aca.CellRect;
                        this.IntializeRendererElement(rendererElement, (S)aca.CellInfo);
                    }
                    VirtualizingCellsControl.SetArrangeCellArgs(rendererElement, aca);
                    SetBounds(rendererElement, aca.CellRect, aca.ForceMeasure, false);
                }
            }
#if MEASURETIME
            }
#endif
        }

        private Rect rectangle;

        private bool isInInitializeRendererElement = false;
        public bool IsInInitializeRendererElement
        {
            get
            {
                return this.isInInitializeRendererElement;
            }
        }

        private void IntializeRendererElement(UIElement rendererElement, GridRenderStyleInfo style)
        {
            if (!this.UseDefaultRenderer)
            {
                return;
            }

            this.isInInitializeRendererElement = true;
            try
            {
                OnInitializeRendererElement(rendererElement, style);
            }
            finally
            {
                this.isInInitializeRendererElement = false;
            }
        }

        protected virtual void OnInitializeRendererElement(UIElement rendererElement, GridRenderStyleInfo style)
        {
            var border = rendererElement as Border;
            if (border == null)
            {
                return;
            }
            // set the background to enable hit-testing in the border element
            border.Background = new SolidColorBrush(Colors.Transparent);
            var textBlock = new TextBlock();
            border.Child = textBlock;
            textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
            TextBlock tb = textBlock;
            if (tb != null)
            {
                var font = style.ReadOnlyFont;
                tb.FontFamily = font.FontFamily;
                tb.FontSize = font.FontSize;
                tb.FontStretch = font.FontStretch;
                tb.FontWeight = font.FontWeight;
                tb.FontStyle = font.FontStyle;
                tb.Foreground = style.Foreground;
                tb.HorizontalAlignment = style.HorizontalAlignment;
                tb.Padding = style.BorderMargins.ToThickness();
#if!WinRT
                tb.TextDecorations = font.TextDecorations;
#endif
                tb.TextWrapping = style.TextTrimming != TextTrimming.WordEllipsis ? style.TextWrapping : TextWrapping.NoWrap;
                tb.TextTrimming = style.TextTrimming;
                tb.VerticalAlignment = style.VerticalAlignment;
                tb.TextAlignment = this.HorizontalAlignmentToTextAlignment(style.HorizontalAlignment);
                Thickness margins = style.TextMargins.ToThickness();
#if!WinRT
                if (style.HasImageIndex)
                {
                    tb.Margin = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
                }
                else
                {
                    tb.Margin = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
                }
#endif
            }
            textBlock.Text = GetControlText(style);
            if (style.Font.Orientation != 0)
            {
                RotateTransform rotate = new RotateTransform();
                // Rect bounds = GetBounds(rectan);
                rotate.Angle = style.Font.Orientation;
                if (!rectangle.IsEmpty)
                {
                    if (style.Font.Orientation <= 90)
                    {
                        rotate.CenterX = rectangle.Height / 2;//rectangle.Width/2;
                        rotate.CenterY = rectangle.Height / 2;//rectangle.Width/2;
                    }
                    else if (style.Font.Orientation <= 180)
                    {
                        rotate.CenterX = rectangle.Width / 2;// - tb.Width;//rectangle.Width/2;
                        rotate.CenterY = rectangle.Height / 2;//rectangle.Height/2;
                    }
                    else if (style.Font.Orientation <= 270)
                    {
                        rotate.CenterX = rectangle.Height / 2;// - tb.Width;//rectangle.Width/2;
                        rotate.CenterY = rectangle.Height / 2;//rectangle.Height/2;
                    }
                    else if (style.Font.Orientation <= 360)
                    {
                        rotate.CenterX = rectangle.Height / 2 + rectangle.Width / 2;// - tb.Width;//rectangle.Width/2;
                        rotate.CenterY = rectangle.Height / 2;//rectangle.Height/2;
                    }
                }
                tb.RenderTransform = rotate;
                Rect rotatedrec = rotate.TransformBounds(rectangle);
                // tb.RenderTransform = rotate;
                SetBounds(tb, rotatedrec);
            }


            VisualContainer.SetWantsMouseInput(textBlock, false);
            VisualContainer.SetWantsMouseInput(border, false);
        }

        protected void ApplyBinding(DependencyObject target, DependencyProperty property, Binding binding)
        {
            if (binding != null)
            {
                BindingOperations.SetBinding(target, property, binding);
            }
        }

        /// <summary>
        /// Set Backgrounds, BorderThickness, Padding and IsEnabled properties.
        /// </summary>
        /// <param name="uiElement"></param>
        protected virtual void InitializeDefaultProperties(T uiElement)
        {
#if !WinRT
#if SILVERLIGHT
            Control c = uiElement as Control;
            if (c == null)
                return;
            if (AllowTransparentBackground)
                c.Background = transparentBrush;
            c.Padding = new Thickness(0);
            c.IsEnabled = true;
#else
            if (AllowTransparentBackground)
                uiElement.SetValue(Control.BackgroundProperty,Brushes.Transparent);
            uiElement.SetValue(Control.PaddingProperty, new Thickness(0));
            uiElement.SetValue(FrameworkElement.IsEnabledProperty, true);
#endif
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
                    var isDelayLoad = GridControlBase.GetDelayLoad(el);
                    if (!isDelayLoad)
                    {
                        InvalidateMeasureRecursive(el);
                        el.Measure(GridUtil.GetSize(rect));
                        OnElementMeasured(el, GridUtil.GetSize(rect));
                    }
                    else
                    {
#if WinRT
                        el.Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                            {
                                InvalidateMeasureRecursive(el);
                                el.Measure(GridUtil.GetSize(rect));
                                OnElementMeasured(el, GridUtil.GetSize(rect));
                                el.Arrange(rect);
                                OnElementArranged(el, rect);
                            });
#else
                        el.Dispatcher.BeginInvoke(new Action(() =>
                        {
                        InvalidateMeasureRecursive(el);
                        el.Measure(GridUtil.GetSize(rect));
                        OnElementMeasured(el, GridUtil.GetSize(rect));
                        el.Arrange(rect);
                        OnElementArranged(el, rect);
                        }));
#endif
                        return;
                    }
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
#if WPF
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

            if (offsetX == 0 && offsetY == 0) return;

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
#if WPF
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
#if WPF
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

        public UIElement GetRendererElement(CellUIElements cellUIElements)
        {
            if (cellUIElements.UIElements.Count == 0)
            {
                return null;
            }

            return cellUIElements.UIElements[0];
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
            /*if (SupportsRenderOptimization && aca.IsProcessingArrangeCellUIElements)
                return;*/
#if MEASURETIME
            //using (MeasureTime.Measure("GridVirtualizingCellRenderer.OnPrepareUIElements")) 
            {
#endif
            var canCreateRenderElement = SupportsRenderOptimization && !aca.ShouldSwapUIElements;
            if (!canCreateRenderElement)
            {
                T uiElement = CreateOrRecycleUIElement(aca, canvas, cellInfo);

                if (!aca.CloneUIElement)
                {
                    // Wire events
                    WireUIElement(uiElement);
                }

                // Also save reference 
                uiElements.Add(uiElement);
            }
            else
            {
                var rendererElement = this.CreateRecycledRendererElement(aca, canvas, cellInfo);
                uiElements.Add(rendererElement);
            }
#if MEASURETIME
            }
#endif
        }

        private UIElement CreateRecycledRendererElement(ArrangeCellArgs aca, ScrollControlChildFrame canvas, GridRenderStyleInfo cellInfo)
        {
            var rendererElement = this.recycleDefaultRendererBin.Dequeue(canvas);
            if (rendererElement != null)
            {
                return rendererElement;
            }

            rendererElement = this.CreateRendererElement(aca, cellInfo);
            return rendererElement;
        }

        protected virtual UIElement CreateRendererElement(ArrangeCellArgs aca, GridRenderStyleInfo cellInfo)
        {
            return new Border();
        }

        T CreateOrRecycleUIElement(ArrangeCellArgs aca, ScrollControlChildFrame canvas, S cellInfo)
        {
            if (aca.CloneUIElement)
            {
                return CreateUIElement(aca, cellInfo);
            }

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
        //#if WinRT
        //        /// <summary>
        //        /// Called from <see cref="ICellRenderer.UnloadUIElements"/> after a cell is scrolled out of view.
        //        /// VirtualizingCellRendererBase overrides this method and
        //        /// creates either removes the cell renderer visuals from the parent canvas
        //        /// or hide them and reuse it later in same canvas depending on whether
        //        /// <see cref="AllowRecycle"/> was set.
        //        /// </summary>
        //        /// <param name="host">The host.</param>
        //        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        //        /// <param name="visuals">The visuals.</param>
        //        protected sealed override void OnUnloadUIElements(RowColumnIndex cellRowColumnIndex, CellUIElements visuals)
        //        {
        //            bool isloaded = true;
        //            if (!AllowRecycle || !isloaded)
        //                visuals.Unload();
        //            else
        //            {
        //                T uiElement = GetUIElement(visuals);
        //                if (uiElement != null)
        //                {
        //#if!WinRT
        //                    uiElement.ClearValue(VisualContainer.CellRenderBoundsProperty);
        //                    if (!AllowRecycleIfIsKeyboardFocusWithin && VirtualizingCellsControl.GetHasFocusWithin(uiElement)) // avoid later trouble because element has focus.
        //                        visuals.Unload();
        //                    else
        //#endif
        //                    {
        //                        if (uiElement.RenderTransform != null)
        //                            uiElement.RenderTransform = null;
        //                        if (uiElement.DataContext != null)
        //                            uiElement.DataContext = null;
        //                        recycleBin.Enqueue(uiElement);
        //                        UnwireUIElement(uiElement);
        //                        visuals.UIElements.Clear();
        //#if (!SILVERLIGHT && !WinRT)
        //                        ScrollControlChildFrame oldCanvas = VisualTreeHelper.GetParent(uiElement) as ScrollControlChildFrame;
        //                        oldCanvas.Children.Remove(uiElement);
        //#endif
        //                    }
        //                }
        //                else
        //                {
        //                    var rendererElement = this.GetRendererElement(visuals);
        //                    if (rendererElement != null)
        //                    {
        //                        rendererElement.ClearValue(VisualContainer.CellRenderBoundsProperty);
        //#if!WinRT
        //                        if (!AllowRecycleIfIsKeyboardFocusWithin)// && VirtualizingCellsControl.GetHasFocusWithin(rendererElement)) // avoid later trouble because element has focus.
        //                            visuals.Unload();
        //                        else
        //#endif
        //                        {
        //                            if (rendererElement.RenderTransform != null)
        //                                rendererElement.RenderTransform = null;
        //                            recycleDefaultRendererBin.Enqueue(rendererElement);
        //                            visuals.Unload();
        //                            visuals.UIElements.Clear();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //#endif
        protected override void OnUnloadUIElements(VirtualizingCellsControl host, RowColumnIndex cellRowColumnIndex, CellUIElements visuals)
        {
            bool isloaded = true;
            if (!AllowRecycle || !isloaded)
                visuals.Unload(host);

            T uiElement = GetUIElement(visuals);
#if WinRT
            if (this is GridCellDataTemplateRenderer)
#else
            if (this is GridCellDataTemplateRenderer || this is GridDataHeaderCellRenderer)
#endif
            {
                if (uiElement != null)
                    UnRegisterUIElement(uiElement);
            }

            if (uiElement != null)
            {
                uiElement.ClearValue(VisualContainer.CellRenderBoundsProperty);
                if (!AllowRecycleIfIsKeyboardFocusWithin && VirtualizingCellsControl.GetHasFocusWithin(uiElement)) // avoid later trouble because element has focus.
                    visuals.Unload(host);
                else
                {
                    if (uiElement.RenderTransform != null)
                        uiElement.RenderTransform = null;
                    if (uiElement.DataContext != null)
                        uiElement.DataContext = null;
                    recycleBin.Enqueue(uiElement);
                    UnwireUIElement(uiElement);
                    visuals.UIElements.Clear();
#if WPF
                        ScrollControlChildFrame oldCanvas = VisualTreeHelper.GetParent(uiElement) as ScrollControlChildFrame;
                        oldCanvas.Children.Remove(uiElement);
#endif
                }
            }
            else
            {
                var rendererElement = this.GetRendererElement(visuals);
                if (rendererElement != null)
                {
                    rendererElement.ClearValue(VisualContainer.CellRenderBoundsProperty);
                    if (!AllowRecycleIfIsKeyboardFocusWithin && VirtualizingCellsControl.GetHasFocusWithin(rendererElement))
                        visuals.Unload(host);
                    else
                    {
                        if (rendererElement.RenderTransform != null)
                            rendererElement.RenderTransform = null;
                        recycleDefaultRendererBin.Enqueue(rendererElement);
                        visuals.UIElements.Clear();
                    }
                }
            }
        }

        bool allowRecycleIfIsKeyboardFocusWithin = false;

        public bool AllowRecycleIfIsKeyboardFocusWithin
        {
            get { return allowRecycleIfIsKeyboardFocusWithin; }
            set { allowRecycleIfIsKeyboardFocusWithin = value; }
        }

#if (SILVERLIGHT || WinRT)
        private void UnwireUIElement(T uiElement)
        {
            uiElement.GotFocus -= new RoutedEventHandler(uiElement_GotFocus);
            uiElement.LostFocus -= new RoutedEventHandler(uiElement_LostFocus);
            OnUnwireUIElement(uiElement);
        }

        internal virtual void UnRegisterUIElement(T uiElement)
        {

        }

        void uiElement_LostFocus(object sender, RoutedEventArgs e)
        {
            VirtualizingCellsControl.SetHasFocusWithin((T)sender, false);
        }

        void uiElement_GotFocus(object sender, RoutedEventArgs e)
        {
            VirtualizingCellsControl.SetHasFocusWithin((T)sender, true);
        }
        private void WireUIElement(T uiElement)
        {
            uiElement.GotFocus += new RoutedEventHandler(uiElement_GotFocus);
            uiElement.LostFocus += new RoutedEventHandler(uiElement_LostFocus);
            OnWireUIElement(uiElement);
        }

        void uiElement_Unloaded(object sender, RoutedEventArgs e)
        {
            T uiElement = (T)sender;
            VirtualizingCellsControl cellsControl = VirtualizingCellsControl.GetCellsControl(uiElement);
            if (cellsControl == null || !cellsControl.ArrangedCellUIElements.Contains(VirtualizingCellsControl.GetCellRowColumnIndex(uiElement)))
                UnwireUIElement(uiElement);
        }
#else

        /// <summary>
        /// Wire events from uiElement
        /// </summary>
        /// <param name="uiElement"></param>
        private void WireUIElement(T uiElement)
        {
            uiElement.Unloaded += new RoutedEventHandler(uiElement_Unloaded);
            OnWireUIElement(uiElement);
        }

        /// <summary>
        /// Unwire previously wired events from uiElement. 
        /// </summary>
        /// <param name="uiElement"></param>
        private void UnwireUIElement(T uiElement)
        {
            uiElement.Unloaded -= new RoutedEventHandler(uiElement_Unloaded);
            OnUnwireUIElement(uiElement);
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
        // Converts a HorizontalAlignment enum to a TextAlignment enum.
        internal virtual TextAlignment HorizontalAlignmentToTextAlignment(HorizontalAlignment horizontalAlignment)
        {
            TextAlignment textAlignment;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                case HorizontalAlignment.Stretch:
                default:
                    textAlignment = TextAlignment.Left;
                    break;

                case HorizontalAlignment.Right:
                    textAlignment = TextAlignment.Right;
                    break;

                case HorizontalAlignment.Center:
                    textAlignment = TextAlignment.Center;
                    break;

            }

            return textAlignment;
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
                CellModel.Dispose();
                recycleBin.Unload();
                recycleDefaultRendererBin.Unload();
            }
        }

        #endregion
    }
    #endregion
}