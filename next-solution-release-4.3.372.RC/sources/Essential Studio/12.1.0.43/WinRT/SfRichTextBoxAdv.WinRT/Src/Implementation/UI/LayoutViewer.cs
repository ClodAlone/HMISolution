#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
#if WPF
using System.Drawing.Printing;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using RangeBaseValueChangedEventHandlerInternal = System.Windows.RoutedPropertyChangedEventHandler<double>;
using RangeBaseValueChangedEventArgsInternal = System.Windows.RoutedPropertyChangedEventArgs<double>;
#else
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Devices.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Input;
using RangeBaseValueChangedEventHandlerInternal = Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventHandler;
using RangeBaseValueChangedEventArgsInternal = Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs;
using Windows.System;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal sealed class TouchEllipse : Grid
    {
        #region Fields
        Ellipse ellipse;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the inner circle diameter.
        /// </summary>
        /// <value>
        /// The inner circle diameter.
        /// </value>
        internal double InnerCircleDiameter
        {
            get
            {
                return ellipse.Width;
            }
            set
            {
                ellipse.Width = ellipse.Height = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TouchEllipse"/> class.
        /// </summary>
        internal TouchEllipse()
        {
            Background = new SolidColorBrush(Colors.Transparent);
#if WPF
            ellipse = new Ellipse() { IsHitTestVisible = false,
#if SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1
            IsManipulationEnabled = false,
#endif
            Fill = new SolidColorBrush(Colors.White), Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 1, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
#else
            ellipse = new Ellipse() { IsHitTestVisible = false, ManipulationMode = ManipulationModes.None, Fill = new SolidColorBrush(Colors.White), Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 1, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
#endif
            Children.Add(ellipse);
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            Children.Clear();
            ellipse = null;
        }
        #endregion
    }
    internal sealed class ImageResizer : Grid
    {
        #region Fields
        internal SfRichTextBoxAdv OwnerControl;
        internal ImageElementBox CurrentImageElementBox;
        Border border;
        FrameworkElement topLeftRect, topMiddleRect, topRightRect, bottomLeftRect, bottomMiddleRect, bottomRightRect, leftMiddleRect, rightMiddleRect;
        HistoryInfo historyInfo = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the size of the resize mark.
        /// </summary>
        /// <value>
        /// The size of the resize mark.
        /// </value>
        internal double ResizeMarkSize
        {
            get
            {
                return topLeftRect.Width;
            }
        }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        internal double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }
            set
            {
                SetWidth(value);
            }
        }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        internal double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            set
            {
                SetHeight(value);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageResizer" /> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        internal ImageResizer(SfRichTextBoxAdv richTextBoxAdv)
        {
            OwnerControl = richTextBoxAdv;
            Background = new SolidColorBrush(Colors.Transparent);
#if WPF
#if SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1
            IsManipulationEnabled = false;
#endif
#else
            ManipulationMode = ManipulationModes.None;
            PointerMoved += ImageResizer_PointerMoved;
#endif
            InitBorder();
            InitTouchResizeMarks();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Sets the width.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetWidth(double value)
        {
            SetValue(WidthProperty, value);
            double gap = value / 2 - topLeftRect.Width;
            if (gap <= 8)
            {
                topMiddleRect.Visibility = Visibility.Collapsed;
                bottomMiddleRect.Visibility = Visibility.Collapsed;
            }
            else
            {
                topMiddleRect.Visibility = Visibility.Visible;
                bottomMiddleRect.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Sets the height.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetHeight(double value)
        {
            SetValue(HeightProperty, value);
            double gap = value / 2 - topLeftRect.Height;
            if (gap <= 8)
            {
                leftMiddleRect.Visibility = Visibility.Collapsed;
                rightMiddleRect.Visibility = Visibility.Collapsed;
            }
            else
            {
                leftMiddleRect.Visibility = Visibility.Visible;
                rightMiddleRect.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Updates the resize mark visibility.
        /// </summary>
        internal void UpdateResizeMarkVisibility()
        {
            double gap = Width / 2 - topLeftRect.Width;
            if (gap <= 8)
            {
                topMiddleRect.Visibility = Visibility.Collapsed;
                bottomMiddleRect.Visibility = Visibility.Collapsed;
            }
            else
            {
                topMiddleRect.Visibility = Visibility.Visible;
                bottomMiddleRect.Visibility = Visibility.Visible;
            }
            gap = Height / 2 - topLeftRect.Height;
            if (gap <= 8)
            {
                leftMiddleRect.Visibility = Visibility.Collapsed;
                rightMiddleRect.Visibility = Visibility.Collapsed;
            }
            else
            {
                leftMiddleRect.Visibility = Visibility.Visible;
                rightMiddleRect.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Initializes the border.
        /// </summary>
        private void InitBorder()
        {
            border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.VerticalAlignment = VerticalAlignment.Stretch;
            Children.Add(border);
        }
        /// <summary>
        /// Initializes the touch resize marks.
        /// </summary>
        internal void InitTouchResizeMarks()
        {
            ClearResizeMarks();
            topLeftRect = InitEllipse(HorizontalAlignment.Left, VerticalAlignment.Top);
            Children.Add(topLeftRect);
            topMiddleRect = InitEllipse(HorizontalAlignment.Center, VerticalAlignment.Top);
            Children.Add(topMiddleRect);
            topRightRect = InitEllipse(HorizontalAlignment.Right, VerticalAlignment.Top);
            Children.Add(topRightRect);
            bottomLeftRect = InitEllipse(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            Children.Add(bottomLeftRect);
            bottomMiddleRect = InitEllipse(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            Children.Add(bottomMiddleRect);
            bottomRightRect = InitEllipse(HorizontalAlignment.Right, VerticalAlignment.Bottom);
            Children.Add(bottomRightRect);
            leftMiddleRect = InitEllipse(HorizontalAlignment.Left, VerticalAlignment.Center);
            Children.Add(leftMiddleRect);
            rightMiddleRect = InitEllipse(HorizontalAlignment.Right, VerticalAlignment.Center);
            Children.Add(rightMiddleRect);
            UpdateResizeMarkVisibility();
        }
        /// <summary>
        /// Initializes the ellipse.
        /// </summary>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        /// <param name="verticalAlignment">The vertical alignment.</param>
        /// <returns></returns>
        private TouchEllipse InitEllipse(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            TouchEllipse ellipse = new TouchEllipse()
#if WPF
            { Width = 25, Height = 25, Margin = new Thickness(-12), InnerCircleDiameter = 13, HorizontalAlignment = horizontalAlignment, VerticalAlignment = verticalAlignment };
#else
            { ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY, Width = 25, Height = 25, Margin = new Thickness(-12), InnerCircleDiameter = 13, HorizontalAlignment = horizontalAlignment, VerticalAlignment = verticalAlignment };
            ellipse.ManipulationDelta += ellipse_ManipulationDelta;
            ellipse.ManipulationCompleted += ellipse_ManipulationCompleted;
            ellipse.PointerPressed += ellipse_PointerPressed;
            ellipse.PointerReleased += ellipse_PointerReleased;
#endif
            return ellipse;
        }
#if !WPF
        /// <summary>
        /// Handles the PointerReleased event of the ellipse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        void ellipse_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            OwnerControl.IsImageResizing = false;
            e.Handled = true;
        }
        /// <summary>
        /// Handles the PointerPressed event of the ellipse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        void ellipse_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            OwnerControl.IsImageResizing = true;
            e.Handled = true;
        }
        /// <summary>
        /// Handles the ManipulationDelta event of the ellipse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ManipulationDeltaRoutedEventArgs"/> instance containing the event data.</param>
        void ellipse_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            double x = e.Delta.Translation.X;
            double y = e.Delta.Translation.Y;
            TouchEllipse ellipse = sender as TouchEllipse;
            switch (ellipse.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    x *= -1;
                    break;
                case HorizontalAlignment.Center:
                    x = 0;
                    break;
            }
            switch (ellipse.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    y *= -1;
                    break;
                case VerticalAlignment.Center:
                    y = 0;
                    break;
            }
            if (x != 0 || y != 0)
            {
                InitHistory(CurrentImageElementBox.ImageContainer);
                CurrentImageElementBox.ImageContainer.Width = CurrentImageElementBox.ImageContainer.Width + x > 0 ? CurrentImageElementBox.ImageContainer.Width + x : 0.1;
                CurrentImageElementBox.ImageContainer.Height = CurrentImageElementBox.ImageContainer.Height + y > 0 ? CurrentImageElementBox.ImageContainer.Height + y : 0.1;
                ParagraphAdv ownerParagraph = CurrentImageElementBox.ImageContainer.OwnerParagraph;
                if (ownerParagraph != null && ownerParagraph.BaseParent != null)
                {
                    ownerParagraph.Relayout(ownerParagraph.Inlines.IndexOf(CurrentImageElementBox.ImageContainer));
                    OwnerControl.Selection.HighlightSelection(false);
                }
            }
            e.Handled = true;
        }
        /// <summary>
        /// Handles the ManipulationCompleted event of the ellipse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ManipulationCompletedRoutedEventArgs"/> instance containing the event data.</param>
        void ellipse_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            UpdateHistory();
            e.Handled = true;
        }
#endif
        /// <summary>
        /// Inits the history.
        /// </summary>
        /// <param name="imageContainer">The image container.</param>
        private void InitHistory(ImageContainerAdv imageContainer)
        {
            if (historyInfo == null)
            {
                historyInfo = new HistoryInfo(OwnerControl);
                historyInfo.Action = Actions.ImageReszing;
                historyInfo.UpdateSelection(OwnerControl.Selection);
                historyInfo.ModifiedProperties.Add(new ImageFormat(imageContainer));
            }
        }
        /// <summary>
        /// Updates the history.
        /// </summary>
        private void UpdateHistory()
        {
            if (historyInfo != null)
            {
                ImageFormat imageFormat = historyInfo.ModifiedProperties[0] as ImageFormat;
                if (CurrentImageElementBox.ImageContainer.Width == imageFormat.Width
                    && CurrentImageElementBox.ImageContainer.Height == imageFormat.Height)
                    historyInfo.ModifiedProperties.Clear();
                else
                    OwnerControl.History.RecordChanges(historyInfo);
                historyInfo = null;
            }
        }
        /// <summary>
        /// Initializes the resize marks.
        /// </summary>
        internal void InitResizeMarks()
        {
            ClearResizeMarks();
            topLeftRect = InitRectangle(HorizontalAlignment.Left, VerticalAlignment.Top);
            Children.Add(topLeftRect);
            topMiddleRect = InitRectangle(HorizontalAlignment.Center, VerticalAlignment.Top);
            Children.Add(topMiddleRect);
            topRightRect = InitRectangle(HorizontalAlignment.Right, VerticalAlignment.Top);
            Children.Add(topRightRect);
            bottomLeftRect = InitRectangle(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            Children.Add(bottomLeftRect);
            bottomMiddleRect = InitRectangle(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            Children.Add(bottomMiddleRect);
            bottomRightRect = InitRectangle(HorizontalAlignment.Right, VerticalAlignment.Bottom);
            Children.Add(bottomRightRect);
            leftMiddleRect = InitRectangle(HorizontalAlignment.Left, VerticalAlignment.Center);
            Children.Add(leftMiddleRect);
            rightMiddleRect = InitRectangle(HorizontalAlignment.Right, VerticalAlignment.Center);
            Children.Add(rightMiddleRect);
            UpdateResizeMarkVisibility();
        }
        /// <summary>
        /// Initializes the rectangle.
        /// </summary>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        /// <param name="verticalAlignment">The vertical alignment.</param>
        /// <returns></returns>
        private Rectangle InitRectangle(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            Rectangle rectangle = new Rectangle()
#if WPF
            { Width = 7, Height = 7, Fill = new SolidColorBrush(Colors.White), Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 1, Margin = new Thickness(-3), HorizontalAlignment = horizontalAlignment, VerticalAlignment = verticalAlignment };
#else
            { ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY, Width = 7, Height = 7, Fill = new SolidColorBrush(Colors.White), Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 1, Margin = new Thickness(-3), HorizontalAlignment = horizontalAlignment, VerticalAlignment = verticalAlignment };
            rectangle.ManipulationDelta += rectangle_ManipulationDelta;
            rectangle.ManipulationCompleted += rectangle_ManipulationCompleted;
            rectangle.PointerPressed += rectangle_PointerPressed;
            rectangle.PointerReleased += rectangle_PointerReleased;
            rectangle.PointerMoved += rectangle_PointerMoved;
#endif
            return rectangle;
        }
#if !WPF
        /// <summary>
        /// Handles the PointerMoved event of the rectangle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        void rectangle_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            CoreCursorType cursorType = CoreCursorType.SizeNortheastSouthwest;
            switch (rectangle.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    switch (rectangle.VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            cursorType = CoreCursorType.SizeNorthwestSoutheast;
                            break;
                        case VerticalAlignment.Center:
                            cursorType = CoreCursorType.SizeWestEast;
                            break;
                    }
                    break;
                case HorizontalAlignment.Center:
                    cursorType = CoreCursorType.SizeNorthSouth;
                    break;
                case HorizontalAlignment.Right:
                    switch (rectangle.VerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            cursorType = CoreCursorType.SizeWestEast;
                            break;
                        case VerticalAlignment.Bottom:
                            cursorType = CoreCursorType.SizeNorthwestSoutheast;
                            break;
                    }
                    break;
            }
            if (!OwnerControl.IsImageResizing)
                OwnerControl.Cursor = new CoreCursor(cursorType, 0);
            e.Handled = true;
        }
        /// <summary>
        /// Handles the PointerReleased event of the rectangle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        void rectangle_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            OwnerControl.IsImageResizing = false;
            e.Handled = true;
        }
        /// <summary>
        /// Handles the PointerPressed event of the rectangle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        void rectangle_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            OwnerControl.IsImageResizing = true;
            e.Handled = true;
        }
        /// <summary>
        /// Handles the ManipulationDelta event of the rectangle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ManipulationDeltaRoutedEventArgs"/> instance containing the event data.</param>
        void rectangle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            double x = e.Delta.Translation.X;
            double y = e.Delta.Translation.Y;
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    x *= -1;
                    break;
                case HorizontalAlignment.Center:
                    x = 0;
                    break;
            }
            switch (rectangle.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    y *= -1;
                    break;
                case VerticalAlignment.Center:
                    y = 0;
                    break;
            }
            if (x != 0 || y != 0)
            {
                InitHistory(CurrentImageElementBox.ImageContainer);
                CurrentImageElementBox.ImageContainer.Width = CurrentImageElementBox.ImageContainer.Width + x > 0 ? CurrentImageElementBox.ImageContainer.Width + x : 0.1;
                CurrentImageElementBox.ImageContainer.Height = CurrentImageElementBox.ImageContainer.Height + y > 0 ? CurrentImageElementBox.ImageContainer.Height + y : 0.1;
                ParagraphAdv ownerParagraph = CurrentImageElementBox.ImageContainer.OwnerParagraph;
                if (ownerParagraph != null && ownerParagraph.BaseParent != null)
                {
                    ownerParagraph.Relayout(ownerParagraph.Inlines.IndexOf(CurrentImageElementBox.ImageContainer));
                    OwnerControl.Selection.HighlightSelection(false);
                }
            }
            e.Handled = true;
        }
        /// <summary>
        /// Handles the ManipulationCompleted event of the rectangle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ManipulationCompletedRoutedEventArgs"/> instance containing the event data.</param>
        void rectangle_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            UpdateHistory();
            e.Handled = true;
        }
        /// <summary>
        /// Handles the PointerMoved event of the ImageResizer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        void ImageResizer_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!OwnerControl.IsImageResizing)
                OwnerControl.Cursor = new CoreCursor(CoreCursorType.SizeAll, 0);
            e.Handled = true;
        }
#endif
        /// <summary>
        /// Clears the resize marks.
        /// </summary>
        private void ClearResizeMarks()
        {
            if (topLeftRect != null)
            {
                Children.Remove(topLeftRect);
                DisposeResizeMarks(topLeftRect);
                topLeftRect = null;
            }
            if (topMiddleRect != null)
            {
                Children.Remove(topMiddleRect);
                DisposeResizeMarks(topMiddleRect);
                topMiddleRect = null;
            }
            if (topRightRect != null)
            {
                Children.Remove(topRightRect);
                DisposeResizeMarks(topRightRect);
                topRightRect = null;
            }
            if (bottomLeftRect != null)
            {
                Children.Remove(bottomLeftRect);
                DisposeResizeMarks(bottomLeftRect);
                bottomLeftRect = null;
            }
            if (bottomMiddleRect != null)
            {
                Children.Remove(bottomMiddleRect);
                DisposeResizeMarks(bottomMiddleRect);
                bottomMiddleRect = null;
            }
            if (bottomRightRect != null)
            {
                Children.Remove(bottomRightRect);
                DisposeResizeMarks(bottomRightRect);
                bottomRightRect = null;
            }
            if (leftMiddleRect != null)
            {
                Children.Remove(leftMiddleRect);
                DisposeResizeMarks(leftMiddleRect);
                leftMiddleRect = null;
            }
            if (rightMiddleRect != null)
            {
                Children.Remove(rightMiddleRect);
                DisposeResizeMarks(rightMiddleRect);
                rightMiddleRect = null;
            }
        }
        /// <summary>
        /// Disposes the resize marks.
        /// </summary>
        /// <param name="element">The element.</param>
        private void DisposeResizeMarks(FrameworkElement element)
        {
            if (element is TouchEllipse)
            {
                (element as TouchEllipse).Dispose();
#if WPF
#else
                element.ManipulationDelta -= ellipse_ManipulationDelta;
                element.PointerPressed -= ellipse_PointerPressed;
                element.PointerReleased -= ellipse_PointerReleased;
#endif
            }
            else
            {
#if WPF
#else
                element.ManipulationDelta -= rectangle_ManipulationDelta;
                element.PointerPressed -= rectangle_PointerPressed;
                element.PointerReleased -= rectangle_PointerReleased;
                element.PointerMoved -= rectangle_PointerMoved;
#endif
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            if (OwnerControl != null)
            {
                if (OwnerControl.Viewer != null && OwnerControl.Viewer.Container != null)
                    OwnerControl.Viewer.Container.Children.Remove(this);
                OwnerControl = null;
            }
            CurrentImageElementBox = null;
#if !WPF
            PointerMoved -= ImageResizer_PointerMoved;
#endif
            ClearResizeMarks();
            Children.Clear();
            border = null;
        }
        #endregion
    }

    internal abstract class LayoutViewer : Canvas
    {
        #region Fields
        internal SfRichTextBoxAdv OwnerControl;
        internal double zoomX = double.NaN;
        internal double zoomY = double.NaN;
        internal Canvas Container;
        internal TranslateTransform transform;
        internal double ScaleFactor = 1;
        internal ScaleTransform scaleTransform = null;
        internal List<PageAdv> AsyncLoadedPages;
        internal double VerticalHeight;
        internal double HorizontalWidth;
        internal ImageResizer ImageResizer;
        internal bool isMousedown = false;
        bool useTouchSelectionMark = true;
        internal bool isTouchDownOnSelectionMark = false;
        internal bool isPageActive = false;
        internal Thickness Padding;
        private PageAdv currentPage, selectionStartPage, selectionEndPage;
        private List<PageAdv> pages;
        private Rectangle caret;
        private Ellipse touchStart;
        private Ellipse touchEnd;
        Storyboard storyBoard;
        DoubleAnimationUsingKeyFrames doubleAnimation;
        private ElementCollection lineElements;
        private Rect clientArea = Rect.Empty;
        private Rect clientActiveArea = Rect.Empty;
        internal HeaderFooter CurrentHeaderFooter = null;
        internal BlockAdv BlockToShift = null;
        internal FieldBeginAdv FieldToLayout = null;
        internal ParagraphAdv FieldEndParagraph = null;
        internal Stack<FieldBeginAdv> FieldStack = null;
        internal bool IsFieldCode = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the visiblebounds.
        /// </summary>
        /// <value>
        /// The visiblebounds.
        /// </value>
        internal Rect Visiblebounds
        {
            get
            {
                if (OwnerControl != null)
                    return new Rect(OwnerControl.Visiblebounds.X, OwnerControl.Visiblebounds.Y, OwnerControl.Visiblebounds.Width, OwnerControl.Visiblebounds.Height);
                return new Rect();
            }
            set
            {
                if (OwnerControl != null)
                    OwnerControl.Visiblebounds = value;
            }
        }
        /// <summary>
        /// Gets or sets the caret.
        /// </summary>
        /// <value>
        /// The caret.
        /// </value>
        internal Rectangle Caret
        {
            get
            {
                return caret;
            }
            set
            {
                caret = value;
            }
        }
        /// <summary>
        /// Gets or sets the touch start mark ellipse.
        /// </summary>
        /// <value>The touch start.</value>
        internal Ellipse TouchStart
        {
            get
            {
                return touchStart;
            }
        }
        /// <summary>
        /// Gets or sets the touch end mark ellipse.
        /// </summary>
        /// <value>The touch end.</value>
        internal Ellipse TouchEnd
        {
            get
            {
                return touchEnd;
            }
        }
        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>
        /// The pages.
        /// </value>
        internal List<PageAdv> Pages
        {
            get
            {
                return pages;
            }
        }
        /// <summary>
        /// Gets the page in which rendering is currently in progress.
        /// </summary>
        internal PageAdv CurrentRenderingPage
        {
            get
            {
                if (pages.Count == 0)
                    return null;
                return pages[pages.Count - 1];
            }
        }
        /// <summary>
        /// Gets the CurrentPage where Cursor available.
        /// </summary>
        internal PageAdv CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                currentPage = value;
            }
        }
        /// <summary>
        /// Gets or sets the selection start page.
        /// </summary>
        /// <value>
        /// The selection start page.
        /// </value>
        private PageAdv SelectionStartPage
        {
            get
            {
                return selectionStartPage;
            }
            set
            {
                if (selectionStartPage != null && selectionStartPage.ForegroundContainer != null)
                    selectionStartPage.ForegroundContainer.Children.Remove(TouchStart);
                selectionStartPage = value;
                if (selectionStartPage != null && selectionStartPage.ForegroundContainer != null)
                    selectionStartPage.ForegroundContainer.Children.Add(TouchStart);
            }
        }
        /// <summary>
        /// Gets or sets the selection end page.
        /// </summary>
        /// <value>
        /// The selection end page.
        /// </value>
        private PageAdv SelectionEndPage
        {
            get
            {
                return selectionEndPage;
            }
            set
            {
                if (selectionEndPage != null && selectionEndPage.ForegroundContainer != null)
                {
                    selectionEndPage.ForegroundContainer.Children.Remove(Caret);
                    selectionEndPage.ForegroundContainer.Children.Remove(TouchEnd);
                }
                selectionEndPage = value;
                if (selectionEndPage != null && selectionEndPage.ForegroundContainer != null)
                {
                    selectionEndPage.ForegroundContainer.Children.Add(Caret);
                    selectionEndPage.ForegroundContainer.Children.Add(TouchEnd);
                }
            }
        }
        /// <summary>
        /// Gets the horizontal scroll bar.
        /// </summary>
        /// <value>
        /// The horizontal scroll bar.
        /// </value>
        internal ScrollBar HorizontalScrollBar
        {
            get
            {
                if (OwnerControl != null)
                    return OwnerControl.horizontalScrollBar;
                return null;
            }
        }
        /// <summary>
        /// Gets the vertical scroll bar.
        /// </summary>
        /// <value>
        /// The vertical scroll bar.
        /// </value>
        internal ScrollBar VerticalScrollBar
        {
            get
            {
                if (OwnerControl != null)
                    return OwnerControl.verticalScrollBar;
                return null;
            }
        }
        /// <summary>
        /// Gets the line elements.
        /// </summary>
        /// <value>
        /// The line elements.
        /// </value>
        internal ElementCollection LineElements
        {
            get
            {
                if (lineElements == null)
                    lineElements = new ElementCollection();
                return lineElements;
            }
        }
        /// <summary>
        /// Gets the client area.
        /// </summary>
        /// <value>
        /// The client area.
        /// </value>
        internal Rect ClientArea
        {
            get
            {
                return clientArea;
            }
        }
        /// <summary>
        /// Gets the client active area.
        /// </summary>
        /// <value>
        /// The client active area.
        /// </value>
        internal Rect ClientActiveArea
        {
            get
            {
                return clientActiveArea;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutViewer"/> class.
        /// </summary>
        /// <param name="richTextBox">The rich text box.</param>
        internal LayoutViewer(SfRichTextBoxAdv richTextBox)
        {
            OwnerControl = richTextBox;
            AsyncLoadedPages = new List<PageAdv>();
#if WPF
            //Handled specifically to avoid focus, on Tab navigation.
            Focusable = false;
#if SyncfusionFramework4_0
            UseLayoutRounding = false;
#endif
#else
            UseLayoutRounding = false;
#endif
            FieldStack = new Stack<FieldBeginAdv>();
            transform = new TranslateTransform();
            scaleTransform = new ScaleTransform();
            pages = new List<PageAdv>();
            InitCaret();
            Container = new Canvas();
            Container.Background = new SolidColorBrush(Colors.Transparent);
            //Adds the container canvas to preserve visible PageAdv items.
            Children.Add(Container);
            ImageResizer = new ImageResizer(OwnerControl) { Visibility = Visibility.Collapsed };
            Canvas.SetZIndex(ImageResizer, 1);
            Container.Children.Add(ImageResizer);
#if !WPF
            //Adds the radial menu.
            if(richTextBox.EnableRadialMenu)
                Children.Add(richTextBox.RadialMenu);
            Canvas.SetZIndex(richTextBox.RadialMenu, 2);
            Children.Add(richTextBox.FontPopup);
            Canvas.SetZIndex(richTextBox.FontPopup, 2);
            Children.Add(richTextBox.CopyIcon);
            Canvas.SetZIndex(richTextBox.CopyIcon, 2);
            InitPrintDocument();
#endif
        }
        #endregion

        #region Layout Implementations
        /// <summary>
        /// Updates the HF client area.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="isHeader">if set to <c>true</c> [is header].</param>
        internal void UpdateHFClientArea(SectionAdv section, bool isHeader)
        {
            double width = section.SectionFormat.PageSize.Width - section.SectionFormat.PageMargin.Left - section.SectionFormat.PageMargin.Right;
            if (width < 0) 
                width = 0;
            if (isHeader)
                clientArea = new Rect(section.SectionFormat.PageMargin.Left, section.SectionFormat.HeaderDistance, width, section.SectionFormat.PageSize.Height - section.SectionFormat.HeaderDistance);
            else
                clientArea = new Rect(section.SectionFormat.PageMargin.Left, section.SectionFormat.PageSize.Height - section.SectionFormat.FooterDistance, width, section.SectionFormat.PageSize.Height - section.SectionFormat.FooterDistance);
            clientActiveArea = clientArea;
        }
        /// <summary>
        /// Updates the client area.
        /// </summary>
        /// <param name="sectionFormat">The section format.</param>
        internal void UpdateClientArea(SectionFormat sectionFormat)
        {
            if (this is FlowLayoutViewer)
            {
                UpdateClientArea();
                return;
            }
            //Updates the top and bottom based on the header footer rendered area.
            Thickness margin = sectionFormat.PageMargin;
            double top = margin.Top;
            if (CurrentRenderingPage.HeaderWidget != null)
                top = Math.Max(sectionFormat.HeaderDistance + CurrentRenderingPage.HeaderWidget.Height, margin.Top);
            double bottom = 0.667 + margin.Bottom;
            if (CurrentRenderingPage.FooterWidget != null)
                bottom = 0.667 + Math.Max(sectionFormat.FooterDistance + CurrentRenderingPage.FooterWidget.Height, margin.Bottom);
            double width = sectionFormat.PageSize.Width - margin.Left - margin.Right;
            if (width < 0)
                width = 0;
            clientArea = new Rect(margin.Left, top, width, sectionFormat.PageSize.Height - top - bottom);
            clientActiveArea = clientArea;
        }
        /// <summary>
        /// Updates the client area, specific for flow layout.
        /// </summary>
        internal void UpdateClientArea()
        {
            if (OwnerControl.ReadLocalValue(SfRichTextBoxAdv.PaddingProperty) == DependencyProperty.UnsetValue)
                Padding = new Thickness(10);
            else
                Padding = OwnerControl.Padding;
            double width = (Visiblebounds.Width - Padding.Left - Padding.Right) / ScaleFactor;
            if (width < 0)
                width = 0;
            clientArea = new Rect(Padding.Left / ScaleFactor, Padding.Top / ScaleFactor,  width, 0);
            clientActiveArea = clientArea;
        }
        /// <summary>
        /// Updates the client area.
        /// </summary>
        /// <param name="tableWidget">The table widget.</param>
        internal void UpdateClientArea(TableWidget tableWidget)
        {
            clientActiveArea.X = clientArea.X = tableWidget.Location.X;
            clientActiveArea.Width = clientArea.Width = tableWidget.Width;
        }
        /// <summary>
        /// Updates the client area.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="beforeLayout">if set to <c>true</c> [before layout].</param>
        internal void UpdateClientArea(BlockAdv block, bool beforeLayout)
        {
            if (beforeLayout)
            {
                if (block is TableAdv && (block as TableAdv).TableWidgets.Count > 0)
                {
                    TableWidget tableWidget = (block as TableAdv).TableWidgets[0] as TableWidget;
                    clientActiveArea.X = clientArea.X = tableWidget.Location.X;
                    clientActiveArea.Width = clientArea.Width = tableWidget.Width;
                    //Updates the location of last item.
                    tableWidget = (block as TableAdv).TableWidgets[(block as TableAdv).TableWidgets.Count - 1] as TableWidget;
                    tableWidget.Location = new Point(clientActiveArea.X, clientActiveArea.Y);
                }
                else
                {
#if !WPF
                     UIDispatcher.Execute(() =>
                        {
#endif
                            clientActiveArea.X = clientArea.X = clientArea.X + block.LeftIndent;
                            double width = clientArea.Width - (block.LeftIndent + block.RightIndent);
                            clientActiveArea.Width = clientArea.Width = width > 0 ? width : 0;
#if !WPF
                        });
#endif
                }
            }
            else
            {
#if !WPF
                UIDispatcher.Execute(() =>
                    {
#endif
                        clientActiveArea.X = clientArea.X = clientArea.X - block.LeftIndent;
                        double width = clientArea.Width + block.LeftIndent + block.RightIndent;
                        clientActiveArea.Width = clientArea.Width = width > 0 ? width : 0;
#if !WPF
                    });
#endif
            }
        }
        /// <summary>
        /// Updates the client area.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="beforeLayout">if set to <c>true</c> [before layout].</param>
        internal void UpdateClientArea(TableRowAdv row, bool beforeLayout)
        {
            TableWidget tableWidget = row.OwnerTable.TableWidgets[row.OwnerTable.TableWidgets.Count - 1] as TableWidget;
            if (beforeLayout)
            {
            }
            else
            {
                clientActiveArea.X = clientArea.X = tableWidget.Location.X;
                clientActiveArea.Width = clientArea.Width = tableWidget.Width;
            }
        }
        /// <summary>
        /// Updates the client area.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="beforeLayout">if set to <c>true</c> [before layout].</param>
        internal void UpdateClientArea(TableCellAdv cell, bool beforeLayout)
        {
            TableRowWidget rowWidget = cell.OwnerRow.TableRowWidgets[cell.OwnerRow.TableRowWidgets.Count - 1];
            TableCellWidget cellWidget = cell.TableCellWidgets[cell.TableCellWidgets.Count - 1];
            if (beforeLayout)
            {
                clientActiveArea.X = clientArea.X = cellWidget.Location.X;
                clientActiveArea.Y = cellWidget.Location.Y;
                clientActiveArea.Width = clientArea.Width = cellWidget.Width;
                if (this is PageLayoutViewer)
                    clientActiveArea.Height = double.PositiveInfinity;
            }
            else
            {
                clientActiveArea.X = clientArea.X = cellWidget.Location.X + cellWidget.Width + cellWidget.Margin.Right;
                if (rowWidget.Location.X + rowWidget.Width - clientArea.X < 0)
                    clientActiveArea.Width = clientArea.Width = 0;
                else
                    clientActiveArea.Width = clientArea.Width = rowWidget.Location.X + rowWidget.Width - clientArea.X;
                clientActiveArea.Y = cellWidget.Location.Y - cellWidget.Margin.Top;
                if (!cell.OwnerTable.IsInsideTable)
                    clientActiveArea.Height = clientArea.Bottom - rowWidget.Location.Y > 0 ? clientArea.Bottom - rowWidget.Location.Y : 0;
            }
        }
        /// <summary>
        /// Updates the width of the client.
        /// </summary>
        /// <param name="width">The width.</param>
        internal void UpdateClientWidth(double width)
        {
            clientActiveArea.X -= width;
            clientActiveArea.Width += width;
        }
        /// <summary>
        /// Cuts from left.
        /// </summary>
        /// <param name="x">The x.</param>
        internal void CutFromLeft(double x)
        {
            if (x < clientActiveArea.Left)
                x = clientActiveArea.Left;
            if (x > clientActiveArea.Right)
                x = clientActiveArea.Right;
            clientActiveArea.Width = (double)(clientActiveArea.Right - x);
            clientActiveArea.X = (double)x;
        }
        /// <summary>
        /// Cuts from top.
        /// </summary>
        /// <param name="y">The y.</param>
        internal void CutFromTop(double y)
        {
            if (y < clientActiveArea.Top)
                y = clientActiveArea.Top;
            if (this is PageLayoutViewer && y > clientActiveArea.Bottom)
                y = clientActiveArea.Bottom;
            if (this is PageLayoutViewer)
                clientActiveArea.Height = (double)(clientActiveArea.Bottom - y);
            else
            {
                clientActiveArea.Height = (double)(y);
                clientArea.Height = clientActiveArea.Height;
            }
            clientActiveArea.X = clientArea.X;
            clientActiveArea.Width = clientArea.Width;
            clientActiveArea.Y = (double)y;
        }
        /// <summary>
        /// Shifts the layouted items.
        /// </summary>
        internal void ShiftLayoutedItems()
        {
            if (BlockToShift == null || BlockToShift.BaseParent == null)
            {
                BlockToShift = null;
                return;
            }
#if DEBUG
            if (OwnerControl.IsDocumentLoaded)
                OwnerControl.PerformanceInfo.RenderingStartTime = DateTime.Now;
#endif
            BlockAdv block = BlockToShift;
            block.RelayoutOrShiftWidgets(this);
            bool updateNextBlockList = true;
            while (block.NextNode is BlockAdv)
            {
                Widget currentWidget = null;
                if (block is ParagraphAdv)
                    currentWidget = (block as ParagraphAdv).ParagraphWidgets[(block as ParagraphAdv).ParagraphWidgets.Count - 1];
                else
                    currentWidget = (block as TableAdv).TableWidgets[(block as TableAdv).TableWidgets.Count - 1];
                block = block.NextNode as BlockAdv;
                updateNextBlockList = false;
                Widget nextWidget = null;
                if (block is ParagraphAdv)
                    nextWidget = (block as ParagraphAdv).ParagraphWidgets[0];
                else
                    nextWidget = (block as TableAdv).TableWidgets[0];
                if (currentWidget.ContainerWidget == nextWidget.ContainerWidget
                    && (Math.Round(nextWidget.Location.Y, 2) == Math.Round(ClientActiveArea.Y, 2)))
                    break;
                updateNextBlockList = true;
                block.RelayoutOrShiftWidgets(this);
            }
            block.UpdateListItemsTillEnd(updateNextBlockList);
            BlockToShift = null;
            UpdateScrollBars();
            if (this is PageLayoutViewer)
                (this as PageLayoutViewer).UpdateVisiblePages(true);
            else
            {
                CurrentPage.RemoveWidgets();
                CurrentPage.RenderWidgets(this as FlowLayoutViewer);
            }
#if DEBUG
            if (OwnerControl.IsDocumentLoaded)
                OwnerControl.PerformanceInfo.RTERenderingTime = DateTime.Now - OwnerControl.PerformanceInfo.RenderingStartTime;
#endif
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Adds the empty page.
        /// </summary>
        internal abstract void AddEmptyPage();
        /// <summary>
        /// Determines whether loading preload pages.
        /// </summary>
        internal abstract bool IsLoadingPreloadPages();
        /// <summary>
        /// Handles the vertical scroll bar value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        internal abstract void VerticalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgsInternal e);
        /// <summary>
        /// Handles the horizontal scroll bar value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        internal abstract void HorizontalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgsInternal e);
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        internal abstract void FindFocusedPage(MouseEventArgs e);
#if !WPF
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        internal abstract void FindFocusedPage(PointerRoutedEventArgs e);
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="TappedRoutedEventArgs"/> instance containing the event data.</param>
        internal abstract void FindFocusedPage(TappedRoutedEventArgs e);
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="DoubleTappedRoutedEventArgs"/> instance containing the event data.</param>
        internal abstract void FindFocusedPage(DoubleTappedRoutedEventArgs e);
        /// <summary>
        /// Updates the core cursor.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        internal abstract void UpdateCoreCursor(PointerRoutedEventArgs e);
#endif
#if WPF && SyncfusionFramework4_0
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        internal abstract void FindFocusedPage(TouchEventArgs e);
#endif
        /// <summary>
        /// Creates the new page.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        internal abstract PageAdv CreateNewPage(SectionAdv section);
        /// <summary>
        /// Zooms this instance.
        /// </summary>
        internal abstract void Zoom();
        /// <summary>
        /// Updates the scroll bars.
        /// </summary>
        internal abstract void UpdateScrollBars();
        /// <summary>
        /// Renders the visible pages.
        /// </summary>
        internal abstract void RenderVisiblePages();
        #endregion

        #region Override Methods
        /// <summary>
        /// Measures the child elements
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!double.IsInfinity(availableSize.Height))
                Visiblebounds = new Rect(Visiblebounds.X, Visiblebounds.Y, Visiblebounds.Width, availableSize.Height);
            if (!OwnerControl.IsLayoutEnabled)
            {
                OwnerControl.IsControlLoaded = true;
                if (OwnerControl.Document == null && OwnerControl.IsDocumentLoaded)
                    OwnerControl.CreateBlockOnEmpty();
                if (OwnerControl.Document != null)
                    OwnerControl.Document.LayoutItems();
            }
            if (OwnerControl.LayoutType == LayoutType.Block)
            {
                double contentHeight = Container.Height;
                availableSize.Height = OwnerControl.ValidateHeight(availableSize.Height, contentHeight);
                Visiblebounds = new Rect(Visiblebounds.X, Visiblebounds.Y, Visiblebounds.Width, availableSize.Height);
            }
            RectangleGeometry rectGeo = new RectangleGeometry();
            rectGeo.Rect = new Rect(0, 0, availableSize.Width, availableSize.Height);
            Clip = rectGeo;
            base.MeasureOverride(availableSize);
            return availableSize;
        }
        #endregion

        #region Pointer events
#if !WPF
        /// <summary>
        /// Raises the <see cref="E:PointerWheelChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        internal void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            if (IsLoadingPreloadPages() || OwnerControl.LayoutType == LayoutType.Block)
            {
                if (OwnerControl.LayoutType != LayoutType.Block)
                    e.Handled = true;
                return;
            }
            e.Handled = true;
            PointerPoint pointerPoint = e.GetCurrentPoint(this);
            if (e.KeyModifiers == VirtualKeyModifiers.Control && OwnerControl.IsZoomEnabled)
            {
                if (pointerPoint.Properties.MouseWheelDelta < 1)
                    ScaleFactor = (ScaleFactor - 0.05 >= 0.10) ? ScaleFactor - 0.05 : 0.10;
                else if (pointerPoint.Properties.MouseWheelDelta > 1)
                    ScaleFactor = (ScaleFactor + 0.05 <= 5) ? ScaleFactor + 0.05 : 5;
                OwnerControl.m_zoomFlag = true;
                OwnerControl.ZoomFactor = Math.Round(ScaleFactor * 100);
                Zoom();
            }
            else
            {
                //Handled for panning/scrolling (Move) the page to view particular region.
                if (pointerPoint.Properties.IsHorizontalMouseWheel)
                    HorizontalScrollBar.Value = HorizontalScrollBar.Value - pointerPoint.Properties.MouseWheelDelta;
                else
                    VerticalScrollBar.Value = VerticalScrollBar.Value - pointerPoint.Properties.MouseWheelDelta;
            }
        }
        /// <summary>
        /// Raises the <see cref="E:ManipulationDelta" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationDeltaRoutedEventArgs" /> instance containing the event data.</param>
        internal void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            if (IsLoadingPreloadPages() || OwnerControl.LayoutType == LayoutType.Block)
            {
                if (OwnerControl.LayoutType != LayoutType.Block)
                    e.Handled = true;
                return;
            }
            //Handled for zooming the page view size.
            if (e.Delta.Scale != 1 && OwnerControl.IsZoomEnabled)
            {
                e.Handled = true;
                if (e.Delta.Scale < 1)
                    ScaleFactor = (ScaleFactor * e.Delta.Scale >= 0.10) ? Math.Round(ScaleFactor * e.Delta.Scale, 2) : 0.10;
                else if (e.Delta.Scale > 1)
                    ScaleFactor = (ScaleFactor * e.Delta.Scale <= 5) ? Math.Round(ScaleFactor * e.Delta.Scale, 2) : 5;
                zoomX = e.Position.X;
                zoomY = e.Position.Y;
                OwnerControl.m_zoomFlag = true;
                OwnerControl.ZoomFactor = Math.Round(ScaleFactor * 100);
                Zoom();
            }
            else if (e.PointerDeviceType != PointerDeviceType.Mouse && !isTouchDownOnSelectionMark)
            {
                e.Handled = true;
                //Handled for panning (Move) the page to view particular region.
                if (e.Delta.Translation.X != 0)
                    this.HorizontalScrollBar.Value = HorizontalScrollBar.Value - e.Delta.Translation.X;
                if (e.Delta.Translation.Y != 0)
                    this.VerticalScrollBar.Value = VerticalScrollBar.Value - e.Delta.Translation.Y;
            }
        }
        /// <summary>
        /// Raises the <see cref="E:PointerMoved" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        internal void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (isMousedown && OwnerControl.FocusState != FocusState.Unfocused)
            {
                FindFocusedPage(e);
                if (CurrentPage != null)
                {
                    PointerPoint pointer = e.GetCurrentPoint(CurrentPage.ForegroundContainer);
                    if (pointer.PointerDevice.PointerDeviceType == PointerDeviceType.Mouse && pointer.Properties.IsLeftButtonPressed
                        || (isTouchDownOnSelectionMark || !useTouchSelectionMark && OwnerControl.LayoutType == LayoutType.Block))
                    {
                        e.Handled = true;
                        double touchY = pointer.Position.Y;
                        if (isTouchDownOnSelectionMark)
                        {
                            PointerPoint touchPointer = e.GetCurrentPoint(TouchEnd);
                            if (touchPointer.Position.Y <= 26)
                                touchY -= touchPointer.Position.Y < 0 ? 0 : touchPointer.Position.Y + 0.5;
                            else
                                touchY -= 36.5;
                        }
                        Point touchPoint = new Point(pointer.Position.X, touchY);
                        OwnerControl.Selection.MoveTextPosition(touchPoint);

                        double y = CurrentPage.BoundingRectangle.Top * ScaleFactor + (Pages.IndexOf(CurrentPage) + 1) * 20 * (1 - ScaleFactor);
                        if (y + pointer.Position.Y * ScaleFactor + 5 > Visiblebounds.Bottom)
                            VerticalScrollBar.Value += 20;
                        if (y + pointer.Position.Y * ScaleFactor - 5 < Visiblebounds.Top)
                            VerticalScrollBar.Value -= 20;

                        double x = (Visiblebounds.Width - HorizontalWidth * ScaleFactor) / 2;
                        if (x < 30)
                            x = 30;
                        if (CurrentPage.BoundingRectangle.Width < HorizontalWidth)
                            x += (HorizontalWidth - CurrentPage.BoundingRectangle.Width) * ScaleFactor / 2;
                        if (x + pointer.Position.X * ScaleFactor + 5 > Visiblebounds.Right)
                            HorizontalScrollBar.Value += HorizontalScrollBar.Maximum;
                        if (x + pointer.Position.X * ScaleFactor - 5 < Visiblebounds.Left)
                            HorizontalScrollBar.Value -= HorizontalScrollBar.Maximum;
                    }
                    CheckForCursorVisibility(true);
                }
            }
            else if (OwnerControl.FocusState != FocusState.Unfocused)
                UpdateCoreCursor(e);
        }
        /// <summary>
        /// Raises the <see cref="E:PointerPressed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        internal void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (OwnerControl.Selection.IsEmpty)
                useTouchSelectionMark = false;
            isMousedown = true;
            FindFocusedPage(e);
            if (OwnerControl.FocusState == FocusState.Unfocused)
                OwnerControl.Focus(FocusState.Pointer);
            //ToDo handle skip if ((Pages.Count < OwnerControl.PreloadPageCount || this is FlowLayoutViewer) && !OwnerControl.IsDocumentLoaded)
            if (CurrentPage != null && OwnerControl.Selection.Start != null)
            {
                PointerPoint pointerPoint = e.GetCurrentPoint(CurrentPage.ForegroundContainer);
                Point touchPoint = pointerPoint.Position;
                if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse)
                {
                    double x = OwnerControl.Selection.Start.Location.X;
                    double y = GetCaretBottom(OwnerControl.Selection.Start, OwnerControl.Selection.IsEmpty) + 9;
                    isTouchDownOnSelectionMark = ((touchPoint.Y <= y && touchPoint.Y >= y - 20 || touchPoint.Y >= y && touchPoint.Y <= y + 20)
                        && (touchPoint.X <= x && touchPoint.X >= x - 20 || touchPoint.X >= x && touchPoint.X <= x + 20));
                    if (!OwnerControl.Selection.IsEmpty)
                    {
                        x = OwnerControl.Selection.End.Location.X;
                        y = GetCaretBottom(OwnerControl.Selection.End, false) + 9;
                        isTouchDownOnSelectionMark = ((touchPoint.Y <= y && touchPoint.Y >= y - 20 || touchPoint.Y >= y && touchPoint.Y <= y + 20)
                            && (touchPoint.X <= x && touchPoint.X >= x - 20 || touchPoint.X >= x && touchPoint.X <= x + 20));
                    }
                }
                if (!isTouchDownOnSelectionMark)
                {
                    bool navigateHyperlink = false;
                    if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse || ((OwnerControl.ModifierKey & VirtualKeyModifiers.Control) == VirtualKeyModifiers.Control
                        && pointerPoint.Properties.IsLeftButtonPressed))
                        navigateHyperlink = true;
                    UpdateTextPosition(new Point(touchPoint.X, touchPoint.Y), navigateHyperlink, false);
                }
                CheckForCursorVisibility(true);
            }
        }
        /// <summary>
        /// Raises the <see cref="E:PointerReleased" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        internal void OnPointerReleased(PointerRoutedEventArgs e)
        {
            isMousedown = false;
            isTouchDownOnSelectionMark = false;
            useTouchSelectionMark = true;
        }
        /// <summary>
        /// Raises the <see cref="E:Tapped" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TappedRoutedEventArgs"/> instance containing the event data.</param>
        internal void OnTapped(TappedRoutedEventArgs e)
        {
            isMousedown = true;
            isPageActive = true;
            FindFocusedPage(e);
            if (OwnerControl.FocusState == FocusState.Unfocused)
                OwnerControl.Focus(FocusState.Pointer);
            if (CurrentPage != null && OwnerControl.Selection.Start != null)
            {
                Point touchPoint = e.GetPosition(CurrentPage.ForegroundContainer);
                if (!isTouchDownOnSelectionMark)
                    UpdateTextPosition(new Point(touchPoint.X, touchPoint.Y), true, false);
                CheckForCursorVisibility(true);
            }
        }
        /// <summary>
        /// Raises the <see cref="E:DoubleTapped" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DoubleTappedRoutedEventArgs"/> instance containing the event data.</param>
        internal void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            isMousedown = false;
            isPageActive = true;
            useTouchSelectionMark = false;
            FindFocusedPage(e);
            if (OwnerControl.FocusState == FocusState.Unfocused)
                OwnerControl.Focus(FocusState.Pointer);
            if (CurrentPage != null && OwnerControl.Selection.Start != null)
            {
                Point touchPoint = e.GetPosition(CurrentPage.ForegroundContainer);
                UpdateTextPosition(new Point(touchPoint.X, touchPoint.Y), false, true);
                CheckForCursorVisibility(true);
            }
        }
#endif
        #endregion

        #region Keyboard Inputs
        /// <summary>
        /// Handles the tab key.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal void HandleTabKey(SelectionAdv selection,bool isNavigateInCell)
        {
            //Perform tab navigation
            TextPosition start = selection.Start;
            if (start != null && start.Paragraph.IsInsideTable && selection.End.Paragraph.IsInsideTable && isNavigateInCell)
            {
                TableCellAdv tablecell = start.Paragraph.AssociatedCell;
                TableRowAdv tableRow = tablecell.OwnerRow;
                TableAdv tableAdv = tableRow.OwnerTable;
                if (tablecell.NextNode == null)
                {
                    if (tableRow.NextNode == null)
                        //Insert new row below
                        selection.InsertRow(RowPlacement.Below);
                    else
                    {
                        //Move text selection or cursor to next row's first cell
                        TableRowAdv nextRow = tableRow.NextNode as TableRowAdv;
                        selection.SelectTableCell(nextRow.Cells[0] as TableCellAdv);
                    }
                }
                else
                {
                    //Move text selection or cursor to next cell in current row
                    selection.SelectTableCell(tablecell.NextNode as TableCellAdv);
                }
            }
            else
            {
                HandleTextInput("\t");
            }
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the shift tab key.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal void HandleShiftTabKey(SelectionAdv selection)
        {
            //Perform tab navigation
            TextPosition start = selection.Start;
            if (start != null && start.Paragraph.IsInsideTable && selection.End.Paragraph.IsInsideTable)
            {
                TableCellAdv tablecell = start.Paragraph.AssociatedCell;
                TableRowAdv tableRow = tablecell.OwnerRow;
                if (tablecell.PreviousNode == null)
                {
                    if (tableRow.PreviousNode != null)
                    {
                        //Move text selection or cursor to previous row's last cell
                        TableRowAdv prevRow = tableRow.PreviousNode as TableRowAdv;
                        selection.SelectTableCell(prevRow.Cells[prevRow.Cells.Count - 1] as TableCellAdv);
                    }
                }
                else
                {
                    //Move text selection or cursor to next cell in current row
                    selection.SelectTableCell(tablecell.PreviousNode as TableCellAdv);
                }
            }
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the shift left key.
        /// </summary>
        internal void HandleShiftLeftKey()
        {
            OwnerControl.Selection.ExtendBackward();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the shift up key.
        /// </summary>
        internal void HandleShiftUpKey()
        {
            OwnerControl.Selection.ExtendToPreviousLine();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the shift right key.
        /// </summary>
        internal void HandleShiftRightKey()
        {
            OwnerControl.Selection.ExtendForward();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the shift down key.
        /// </summary>
        internal void HandleShiftDownKey()
        {
            OwnerControl.Selection.ExtendToNextLine();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the control home key.
        /// </summary>
        internal void HandleControlHomeKey()
        {
            TextPosition documentStart = null;
            if (OwnerControl.Document != null)
                documentStart = OwnerControl.Document.DocumentStart;
            if (documentStart != null)
                OwnerControl.Selection.Select(documentStart);
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the control shift home key.
        /// </summary>
        internal void HandleControlShiftHomeKey()
        {
            TextPosition documentStart = null;
            if (OwnerControl.Document != null)
                documentStart = OwnerControl.Document.DocumentStart;
            if (documentStart != null)
            {
                OwnerControl.Selection.End.SetPosition(documentStart);
                OwnerControl.Selection.FireSelectionChanged(true);
            }
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the control shift end key.
        /// </summary>
        internal void HandleControlShiftEndKey()
        {
            TextPosition documentEnd = null;
            if (OwnerControl.Document != null)
                documentEnd = OwnerControl.Document.DocumentEnd;
            if (documentEnd != null)
            {
                OwnerControl.Selection.End.SetPosition(documentEnd.Paragraph, false);
                OwnerControl.Selection.FireSelectionChanged(true);
            }
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the control end key.
        /// </summary>
        internal void HandleControlEndKey()
        {
            TextPosition documentEnd = null;
            if (OwnerControl.Document != null)
                documentEnd = OwnerControl.Document.DocumentEnd;
            if (documentEnd != null)
                OwnerControl.Selection.Select(documentEnd);
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the control left key.
        /// </summary>
        internal void HandleControlLeftKey()
        {
            if (!OwnerControl.Selection.IsEmpty)
            {
                TextPosition textPosition = OwnerControl.Selection.End;
                if (OwnerControl.Selection.IsForward)
                    textPosition = OwnerControl.Selection.Start;
                OwnerControl.Selection.Select(textPosition);
            }
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the control right key.
        /// </summary>
        internal void HandleControlRightKey()
        {
            if (!OwnerControl.Selection.IsEmpty)
            {
                TextPosition textPosition = OwnerControl.Selection.Start;
                if (OwnerControl.Selection.IsForward)
                    textPosition = OwnerControl.Selection.End;
                OwnerControl.Selection.Select(textPosition);
            }
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the left key.
        /// </summary>
        internal void HandleLeftKey()
        {
            OwnerControl.Selection.MovePreviousPosition();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the right key.
        /// </summary>
        internal void HandleRightKey()
        {
            OwnerControl.Selection.MoveNextPosition();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles up key.
        /// </summary>
        internal void HandleUpKey()
        {
            OwnerControl.Selection.MoveUp();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles down key.
        /// </summary>
        internal void HandleDownKey()
        {
            OwnerControl.Selection.MoveDown();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the text input.
        /// </summary>
        /// <param name="text">The text.</param>
        internal void HandleTextInput(string text)
        {
            OwnerControl.Selection.InsertText(text);
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the enter key.
        /// </summary>
        internal void HandleEnterKey()
        {
            OwnerControl.Selection.OnEnter();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the delete key.
        /// </summary>
        internal void HandleDeleteKey()
        {
            OwnerControl.Selection.OnDelete();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the back key.
        /// </summary>
        internal void HandleBackKey()
        {
            OwnerControl.Selection.OnBackspace();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the home key.
        /// </summary>
        internal void HandleHomeKey()
        {
            OwnerControl.Selection.MoveToLineStart();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the end key.
        /// </summary>
        internal void HandleEndKey()
        {
            OwnerControl.Selection.MoveToLineEnd();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the shift home key.
        /// </summary>
        internal void HandleShiftHomeKey()
        {
            OwnerControl.Selection.ExtendToLineStart();
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Handles the shift end key.
        /// </summary>
        internal void HandleShiftEndKey()
        {
            OwnerControl.Selection.ExtendToLineEnd();
            CheckForCursorVisibility(false);
        }
        #endregion

        #region Update Text Position
        /// <summary>
        /// Gets the line widget.
        /// </summary>
        /// <param name="cursorPoint">The cursor point.</param>
        /// <returns></returns>
        internal LineWidget GetLineWidget(Point cursorPoint)
        {
            LineWidget widget = null;
            if (currentPage != null)
            {
                for (int i = 0; i < currentPage.BodyWidgets.Count; i++)
                {
                    BodyWidget bodyWidget = currentPage.BodyWidgets[i];
                    widget = bodyWidget.GetLineWidget(cursorPoint);
                    if (widget != null)
                        break;
                }
            }
            return widget;
        }
        /// <summary>
        /// Updates the text position.
        /// </summary>
        /// <param name="cursorPoint">The cursor point.</param>
        /// <param name="navigateHyperlink">if set to <c>true</c> [navigate hyperlink].</param>
        /// <param name="isDoubleTapped">if set to <c>true</c> [is double tapped].</param>
        private void UpdateTextPosition(Point cursorPoint, bool navigateHyperlink, bool isDoubleTapped)
        {
            //Updates the text position based on the cursor position.
            LineWidget widget = GetLineWidget(cursorPoint);
            if (widget != null)
            {
                FieldBeginAdv hyperlinkField = null;
                if (navigateHyperlink)
                    hyperlinkField = widget.GetHyperlinkField(OwnerControl, cursorPoint);
                if (hyperlinkField != null)
                {
                    widget.UpdateTextPosition(OwnerControl, cursorPoint, false);
                    //Invokes Hyperlink navigation events.
                    OwnerControl.FireRequestNavigate(hyperlinkField);
                }
                else
                    widget.UpdateTextPosition(OwnerControl, cursorPoint, isDoubleTapped);
            }
        }
        /// <summary>
        /// Moves the cursor to the specified position
        /// </summary>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPosition">The end position.</param>
        internal void ScrollToPosition(TextPosition startPosition, TextPosition endPosition)
        {
            LineWidget lineWidget = endPosition.Paragraph.GetLineWidget(endPosition.Offset);
            double top = lineWidget.GetTop();
            double height = lineWidget.Height;
            //Gets current page.
            PageAdv endPage = (this is FlowLayoutViewer) ? CurrentPage : lineWidget.ParagraphWidget.GetPage();
            double x = 0, y = 0;
            if (this is PageLayoutViewer)
            {
                x = (Visiblebounds.Width - HorizontalWidth * ScaleFactor) / 2;
                if (x < 30)
                    x = 30;
                if (endPage.BoundingRectangle.Width < HorizontalWidth)
                    x += (HorizontalWidth - endPage.BoundingRectangle.Width) * ScaleFactor / 2;
                y = endPage.BoundingRectangle.Top * ScaleFactor + (Pages.IndexOf(endPage) + 1) * 20 * (1 - ScaleFactor);
            }
            x += endPosition.Location.X * ScaleFactor;
            y += endPosition.Location.Y * ScaleFactor;
            //Updates vertical scrollbar.
            if (Visiblebounds.Y > y)
                VerticalScrollBar.Value -= Visiblebounds.Y - y;
            else if (Visiblebounds.Bottom < y + height)
                VerticalScrollBar.Value += y + height - Visiblebounds.Bottom;
            else
                UpdateCaretToPage(startPosition, endPage);
            //Updates horizontal scrollbar.
            if (Visiblebounds.X > x)
                HorizontalScrollBar.Value -= Visiblebounds.Width;
            else if (Visiblebounds.Right < x)
                HorizontalScrollBar.Value += Visiblebounds.Width;
#if !WPF
            if (Visiblebounds.Width >= 2 * OwnerControl.RadialMenu.RadiusX
                && Visiblebounds.Height >= 2 * OwnerControl.RadialMenu.RadiusY)
            {
                double radialLeft = Visiblebounds.Width - 2 * OwnerControl.RadialMenu.RadiusX;
                if (x + 200 < radialLeft)
                    radialLeft = x + 200;
                double radialTop = 0;
                double caretHeight = caret.Height * ScaleFactor;
                if (y - Visiblebounds.Y + caretHeight / 2 + OwnerControl.RadialMenu.RadiusY > Visiblebounds.Height)
                    radialTop = Visiblebounds.Height - 2 * OwnerControl.RadialMenu.RadiusY;
                else if (y - Visiblebounds.Y + caretHeight / 2 - OwnerControl.RadialMenu.RadiusY > 0)
                    radialTop = y - Visiblebounds.Y + caretHeight / 2 - OwnerControl.RadialMenu.RadiusY;
                if (x >= radialLeft)
                {
                    if (Visiblebounds.Bottom >= y + caretHeight + 2 * OwnerControl.RadialMenu.RadiusY)
                        radialTop = y - Visiblebounds.Y + caretHeight;
                    else
                        radialTop = y - Visiblebounds.Y - 2 * OwnerControl.RadialMenu.RadiusY;
                }
                Canvas.SetLeft(OwnerControl.RadialMenu, radialLeft);
                Canvas.SetTop(OwnerControl.RadialMenu, radialTop);
                Canvas.SetLeft(OwnerControl.FontPopup, radialLeft);
                Canvas.SetTop(OwnerControl.FontPopup, radialTop);
            }
#endif
        }
        #endregion

        #region Caret Implementation
        /// <summary>
        /// Updates the caret to page.
        /// </summary>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPage">The end page.</param>
        private void UpdateCaretToPage(TextPosition startPosition, PageAdv endPage)
        {
            if (endPage != null)
            {
                SelectionEndPage = endPage;
                if (OwnerControl.Selection.IsEmpty)
                    SelectionStartPage = endPage;
                else
                {
                    LineWidget startLineWidget = startPosition.Paragraph.GetLineWidget(startPosition.Offset);
                    //Gets start page.
                    PageAdv startPage = startLineWidget.ParagraphWidget.GetPage();
                    if (startPage != null)
                        SelectionStartPage = startPage;
                }
            }
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Updates the caret to page.
        /// </summary>
        internal void UpdateCaretToPage()
        {
            LineWidget lineWidget = null;
            if (OwnerControl.Selection.End != null)
                lineWidget = OwnerControl.Selection.End.Paragraph.GetLineWidget(OwnerControl.Selection.End.Offset);
            //Gets current page.
            if (lineWidget == null)
                return;
            PageAdv endPage = lineWidget.ParagraphWidget.GetPage();
            if (endPage != null)
            {
                SelectionEndPage = endPage;
                if (OwnerControl.Selection.IsEmpty)
                    SelectionStartPage = endPage;
                else
                {
                    LineWidget startLineWidget = OwnerControl.Selection.Start.Paragraph.GetLineWidget(OwnerControl.Selection.Start.Offset);
                    if (startLineWidget != null)
                    {
                        //Gets start page.
                        PageAdv startPage = startLineWidget.ParagraphWidget.GetPage();
                        if (startPage != null)
                            SelectionStartPage = startPage;
                    }
                }
            }
            CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Gets the caret bottom.
        /// </summary>
        /// <param name="textPosition">The text position.</param>
        /// <param name="isEmptySelection">if set to <c>true</c> [is empty selection].</param>
        /// <returns></returns>
        private double GetCaretBottom(TextPosition textPosition, bool isEmptySelection)
        {
            double bottom = textPosition.Location.Y;
            if (textPosition.Paragraph.IsEmpty())
            {
                ParagraphAdv paragraph = textPosition.Paragraph;
                double topMargin = 0, bottomMargin = 0;
                bottom += paragraph.GetParagraphMarkSize(ref topMargin, ref bottomMargin).Height;
                bottom += topMargin;
                if (!isEmptySelection)
                    bottom += bottomMargin;
            }
            else
            {
                int index = 0;
                Inline inline = textPosition.Paragraph.GetInline(textPosition.Offset, ref index);
                double topMargin = 0;
                bool isItalic = false;
                bottom += inline.GetCaretHeight(index, inline.CharacterFormat, false, ref topMargin, ref isItalic);
            }
            return bottom;
        }
        /// <summary>
        /// Updates the size of the caret.
        /// </summary>
        /// <param name="textPosition">The text position.</param>
        /// <returns></returns>
        private double UpdateCaretSize(TextPosition textPosition)
        {
            double topMargin = 0;
            bool isItalic = false;
            if (textPosition.Paragraph.IsEmpty())
            {
                ParagraphAdv paragraph = textPosition.Paragraph;
                double bottomMargin = 0;
                double height = paragraph.GetParagraphMarkSize(ref topMargin, ref bottomMargin).Height;
                Caret.Height = topMargin < 0 ? topMargin + height : height;
                isItalic = paragraph.CharacterFormat.Italic;
            }
            else
            {
                int index = 0;
                Inline inline = textPosition.Paragraph.GetInline(textPosition.Offset, ref index);
                isItalic = inline.CharacterFormat.Italic;
                Caret.Height = inline.GetCaretHeight(index, inline.CharacterFormat, true, ref topMargin, ref isItalic);
            }
            if (isItalic)
            {
                RotateTransform rotate = new RotateTransform();
                rotate.Angle = 13;
                rotate.CenterY = Caret.Height / 2;
                rotate.CenterX = Caret.Width / 2;
                Caret.RenderTransform = rotate;
            }
            else
                Caret.ClearValue(Rectangle.RenderTransformProperty);
            return topMargin;
        }
        /// <summary>
        /// Updates the caret position.
        /// </summary>
        internal void UpdateCaretPosition()
        {
            //Sets the caret position based on the selection end.
            if (OwnerControl.Selection.End != null)
            {
                Point caretPosition = OwnerControl.Selection.End.Location;
                Canvas.SetLeft(caret, Math.Round(caretPosition.X));
                double topMargin = UpdateCaretSize(OwnerControl.Selection.End);
                Canvas.SetTop(caret, caretPosition.Y + topMargin);

                Canvas.SetLeft(touchStart, Math.Round(caretPosition.X - touchStart.Width / 2));
                Canvas.SetTop(touchStart, caretPosition.Y + caret.Height);
                Canvas.SetLeft(touchEnd, Math.Round(caretPosition.X - touchEnd.Width / 2));
                Canvas.SetTop(touchEnd, caretPosition.Y + caret.Height);
            }
        }
        /// <summary>
        /// Updates the touch mark position.
        /// </summary>
        internal void UpdateTouchMarkPosition()
        {
            if (ImageResizer.Visibility == Visibility.Visible)
                return;
            double y = GetCaretBottom(OwnerControl.Selection.Start, false);
            Canvas.SetLeft(touchStart, Math.Round(OwnerControl.Selection.Start.Location.X - touchStart.Width / 2));
            Canvas.SetTop(touchStart, y);
            y = GetCaretBottom(OwnerControl.Selection.End, false);
            Canvas.SetLeft(touchEnd, Math.Round(OwnerControl.Selection.End.Location.X - touchEnd.Width / 2));
            Canvas.SetTop(touchEnd, y);
#if !WPF
            if (OwnerControl.LayoutType == LayoutType.Block)
            {
                y = OwnerControl.Selection.End.Location.Y;
                double x = OwnerControl.Selection.End.Location.X;
                y = y > OwnerControl.CopyIcon.Height ? y - OwnerControl.CopyIcon.Height : 0;
                x = x > OwnerControl.CopyIcon.Width ? x - OwnerControl.CopyIcon.Width : 0;
                if (Visiblebounds.Width <= OwnerControl.CopyIcon.Width)
                    x = 0;
                if (Visiblebounds.Height <= OwnerControl.CopyIcon.Height)
                    y = 0;
                OwnerControl.IsPopItemVisible = true;
                OwnerControl.CopyIcon.Visibility = Visibility.Visible;
                Canvas.SetLeft(OwnerControl.CopyIcon, x);
                Canvas.SetTop(OwnerControl.CopyIcon, y);
            }
#endif
        }
        /// <summary>
        /// Shows the caret.
        /// </summary>
        internal void ShowCaret(bool isTouch)
        {
            if (!OwnerControl.IsReadOnlyMode || OwnerControl.EnableCursorOnReadOnly)
            {
                if (OwnerControl.Selection.IsEmpty)
                    Caret.Visibility = Visibility.Visible;
                else
                {
                    if (Caret.Visibility == Visibility.Visible)
                        Caret.Visibility = Visibility.Collapsed;
                    OwnerControl.SelectionBrush.ClearValue(Brush.OpacityProperty);
                }
                if (OwnerControl.IsTouchInput && ImageResizer.Visibility == Visibility.Collapsed)
                {
                    touchStart.Visibility = Visibility.Visible;
                    touchEnd.Visibility = Visibility.Visible;
                }
                else
                {
                    touchStart.Visibility = Visibility.Collapsed;
                    touchEnd.Visibility = Visibility.Collapsed;
                }
#if !WPF
                if (Visiblebounds.Width >= 3 * OwnerControl.RadialMenu.RadiusX
                    && Visiblebounds.Height >= 3 * OwnerControl.RadialMenu.RadiusY)
                {
                    if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded || ImageResizer.Visibility == Visibility.Visible)
                        OwnerControl.RadialMenu.Visibility = Visibility.Collapsed;
                    else
                    {
                        if (OwnerControl.IsPopItemVisible)
                            OwnerControl.FontPopup.Visibility = Visibility.Visible;
                        else
                            OwnerControl.RadialMenu.Visibility = Visibility.Visible;
                    }
                }
#endif
            }
            else if (OwnerControl.LayoutType == LayoutType.Block)
            {
                if (!OwnerControl.Selection.IsEmpty)
                {
                    if (OwnerControl.IsTouchInput)
                    {
                        touchStart.Visibility = Visibility.Visible;
                        touchEnd.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        touchStart.Visibility = Visibility.Collapsed;
                        touchEnd.Visibility = Visibility.Collapsed;
                    }
#if !WPF
                    if (OwnerControl.IsPopItemVisible)
                        OwnerControl.CopyIcon.Visibility = Visibility.Visible;
#endif
                }
                else
                {
                    touchStart.Visibility = Visibility.Collapsed;
                    touchEnd.Visibility = Visibility.Collapsed;
#if !WPF
                    OwnerControl.IsPopItemVisible = false;
                    OwnerControl.CopyIcon.Visibility = Visibility.Collapsed;
#endif
                }
                OwnerControl.SelectionBrush.ClearValue(Brush.OpacityProperty);
            }
        }
        /// <summary>
        /// Hides the caret
        /// </summary>
        internal void HideCaret(bool isTouch)
        {
            if (OwnerControl.Selection.IsEmpty)
                Caret.Visibility = Visibility.Collapsed;
            else
                OwnerControl.SelectionBrush.Opacity = 0;
            touchStart.Visibility = Visibility.Collapsed;
            touchEnd.Visibility = Visibility.Collapsed;
            isMousedown = false;
            isTouchDownOnSelectionMark = false;
            useTouchSelectionMark = true;
#if !WPF
            OwnerControl.RadialMenu.Visibility = Visibility.Collapsed;
            OwnerControl.FontPopup.Visibility = Visibility.Collapsed;
            OwnerControl.CopyIcon.Visibility = Visibility.Collapsed;
#endif
        }
        #endregion

        #region Image Resizer
        /// <summary>
        /// Shows the image resizer.
        /// </summary>
        internal void ShowImageResizer()
        {
            ImageResizer.Visibility = Visibility.Visible;
#if !WPF
            OwnerControl.RadialMenu.Visibility = Visibility.Collapsed;
#endif
        }
        /// <summary>
        /// Positions the image resizer.
        /// </summary>
        /// <param name="elementBox">The element box.</param>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPosition">The end position.</param>
        internal void PositionImageResizer(ImageElementBox elementBox, TextPosition startPosition, TextPosition endPosition)
        {
            //Adds image resizer to container canvas, if it does not exists.
            if (!Container.Children.Contains(ImageResizer))
                Container.Children.Add(ImageResizer);
            ImageResizer.Width = elementBox.ImageContainer.Width * ScaleFactor;
            ImageResizer.Height = elementBox.ImageContainer.Height * ScaleFactor;
            ImageResizer.CurrentImageElementBox = elementBox;
            LineWidget lineWidget = elementBox.CurrentLineWidget;
            double top = lineWidget.GetTop() + elementBox.Margin.Top;
            double left = lineWidget.GetLeft(elementBox, 0);
            //Gets current page.
            PageAdv page = (this is FlowLayoutViewer) ? CurrentPage : lineWidget.ParagraphWidget.GetPage();
            double x = 0, y = 0;
            if (this is PageLayoutViewer)
            {
                x = (Visiblebounds.Width - HorizontalWidth * ScaleFactor) / 2;
                if (x < 30)
                    x = 30;
                if (page.BoundingRectangle.Width < HorizontalWidth)
                    x += (HorizontalWidth - page.BoundingRectangle.Width) * ScaleFactor / 2;
                y = page.BoundingRectangle.Top * ScaleFactor + (Pages.IndexOf(page) + 1) * 20 * (1 - ScaleFactor);
            }
            double margin = (ImageResizer.ResizeMarkSize - 1) / 2;
            double width = ImageResizer.Width + 2 * margin, height = ImageResizer.Height + 2 * margin;
            if (width > page.Width - left * ScaleFactor + margin)
                width = page.Width - left * ScaleFactor;
            if (height > page.Height - top * ScaleFactor + margin)
                height = page.Height - top * ScaleFactor;
            ImageResizer.ClearValue(ClipProperty);
            if (width < ImageResizer.Width + margin || height < ImageResizer.Height + margin)
            {
                ImageResizer.Clip = new RectangleGeometry();
#if WPF
                (ImageResizer.Clip as RectangleGeometry).Rect = new Rect(0, 0, width, height);
#else
                ImageResizer.Clip.Rect = new Rect(-margin, -margin, width + margin, height + margin);
#endif
            }
            x += left * ScaleFactor;
            y += top * ScaleFactor;
            Canvas.SetLeft(ImageResizer, x);
            Canvas.SetTop(ImageResizer, y);
        }
        /// <summary>
        /// Hides the image resizer.
        /// </summary>
        internal void HideImageResizer()
        {
            ImageResizer.Visibility = Visibility.Collapsed;
#if !WPF
            if (!OwnerControl.IsReadOnlyMode && OwnerControl.IsDocumentLoaded && OwnerControl.IsChildFocused())
                OwnerControl.RadialMenu.Visibility = Visibility.Visible;
#endif
        }
        #endregion

        #region Print Document
#if !WPF
        /// <summary>
        /// Marker interface for document source
        /// </summary>
        private IPrintDocumentSource printDocumentSource = null;
        bool isPrinterRegistered;
        /// <summary>
        /// Prints the document.
        /// </summary>
        internal void PrintDocument()
        {
            //if (this is FlowLayoutViewer)
            //{
            //    //Todo: Handle a temparory page by page layout and print the contents page by page.
            //    return;
            //}
            UIDispatcher.Execute(async () =>
            {
                RegisterForPrinting();
                await PrintManager.ShowPrintUIAsync();
            });
        }
        /// <summary>
        /// Initializes the print document.
        /// </summary>
        private void InitPrintDocument()
        {
            PrintDocument printDocument = OwnerControl.PrintDocumentInternal;
            if (printDocument != null)
            {
                // Save the print document source.
                printDocumentSource = printDocument.DocumentSource;
                // Add an event handler which creates preview pages.
                printDocument.Paginate += CreatePrintPreviewPages;
                // Add an event handler which provides a specified preview page.
                printDocument.GetPreviewPage += GetPrintPreviewPage;
                // Add an event handler which provides all final print pages.
                printDocument.AddPages += AddPrintPages;
            }
        }
        /// <summary>
        /// Disposes the print document.
        /// </summary>
        private void DisposePrintDocument()
        {
            PrintDocument printDocument = OwnerControl.PrintDocumentInternal;
            if (printDocument != null)
            {
                // Removes event handler which creates preview pages.
                printDocument.Paginate -= CreatePrintPreviewPages;
                // Removes event handler which provides a specified preview page.
                printDocument.GetPreviewPage -= GetPrintPreviewPage;
                // Removes event handler which provides all final print pages.
                printDocument.AddPages -= AddPrintPages;
            }
            printDocumentSource = null;
        }
        /// <summary>
        /// Unregisters printing.
        /// </summary>
        internal void UnRegisterPrinting()
        {
            isPrinterRegistered = false;
            // Gets current PrintManager and removes event handler for print task.
            PrintManager printMananger = PrintManager.GetForCurrentView();
            printMananger.PrintTaskRequested -= PrintTaskRequested;
        }
        /// <summary>
        /// Registers for printing.
        /// </summary>
        internal void RegisterForPrinting()
        {
            if (isPrinterRegistered)
                UnRegisterPrinting();
            // Create a PrintManager and add a handler for printing initialization.
            PrintManager printMananger = PrintManager.GetForCurrentView();
            printMananger.PrintTaskRequested += PrintTaskRequested;
            isPrinterRegistered = true;
        }
        /// <summary>
        /// Handles print task requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="PrintTaskRequestedEventArgs"/> instance containing the event data.</param>
        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            PrintTask printTask = args.Request.CreatePrintTask("Syncfusion RichTextEditor Control", sourceRequested => sourceRequested.SetSource(printDocumentSource));
            printTask.Completed += printTask_Completed;
        }
        /// <summary>
        /// Handles print task completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="PrintTaskCompletedEventArgs"/> instance containing the event data.</param>
        void printTask_Completed(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            UIDispatcher.Execute(() =>
            {
                if (isPrinterRegistered)
                {
                    UnRegisterPrinting();
                    PrintCompletedEventArgs printCompletedEventArgs = new PrintCompletedEventArgs();
                    OwnerControl.FirePrintCompleted(printCompletedEventArgs);
                }
            });
        }
        /// <summary>
        /// Adds the print pages.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AddPagesEventArgs"/> instance containing the event data.</param>
        private void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            PrintDocument printDocument = sender as PrintDocument;
            // Loop over all of the preview pages and add each one to  add each page to be printed
            for (int i = 0; i < Pages.Count; i++)
            {
                if (this is PageLayoutViewer)
                {
                    if ((this as PageLayoutViewer).VisiblePages.Contains(Pages[i]))
                    {
                        //Sets the page to normal (100%) scaling factor.
                        Pages[i].Width = Pages[i].BoundingRectangle.Width;
                        Pages[i].Height = Pages[i].BoundingRectangle.Height;
                        Pages[i].ForegroundContainer.ClearValue(Canvas.RenderTransformProperty);
                        Pages[i].DecorationContainer.ClearValue(Canvas.RenderTransformProperty);
                        Pages[i].HidePageNumber();
                    }
                    else
                        Pages[i].RenderWidgets();
                }
                UpdateCaret(Pages[i], true);
                // We should have all pages ready at this point...
                printDocument.AddPage(Pages[i]);
                if (this is PageLayoutViewer)
                {
                    if ((this as PageLayoutViewer).VisiblePages.Contains(Pages[i]))
                    {
                        //Resets the page to current scaling factor.
                        Pages[i].Width = Pages[i].BoundingRectangle.Width * scaleTransform.ScaleX;
                        Pages[i].Height = Pages[i].BoundingRectangle.Height * scaleTransform.ScaleY;
                        Pages[i].ForegroundContainer.RenderTransform = scaleTransform;
                        Pages[i].DecorationContainer.RenderTransform = scaleTransform;
                    }
                    else
                        Pages[i].RemoveWidgets();
                }
                UpdateCaret(Pages[i], false);
            }

            // Indicate that all of the print pages have been provided
            printDocument.AddPagesComplete();
        }
        /// <summary>
        /// Gets the print preview page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="GetPreviewPageEventArgs"/> instance containing the event data.</param>
        private void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            PrintDocument printDocument = sender as PrintDocument;
            int currentPreviewPage = 0;
            Interlocked.Exchange(ref currentPreviewPage, e.PageNumber - 1);

            PageAdv page = Pages[e.PageNumber - 1];
            if (this is PageLayoutViewer)
            {
                if ((this as PageLayoutViewer).VisiblePages.Contains(page))
                {
                    //Sets the page to normal (100%) scaling factor.
                    page.Width = page.BoundingRectangle.Width;
                    page.Height = page.BoundingRectangle.Height;
                    page.ForegroundContainer.ClearValue(Canvas.RenderTransformProperty);
                    page.DecorationContainer.ClearValue(Canvas.RenderTransformProperty);
                    page.HidePageNumber();
                }
                else
                    page.RenderWidgets();
            }
            UpdateCaret(page, true);
            // Set the preview even if images failed to load properly
            printDocument.SetPreviewPage(e.PageNumber, page);
            if (this is PageLayoutViewer)
            {
                if ((this as PageLayoutViewer).VisiblePages.Contains(page))
                {
                    //Resets the page to current scaling factor.
                    page.Width = page.BoundingRectangle.Width * scaleTransform.ScaleX;
                    page.Height = page.BoundingRectangle.Height * scaleTransform.ScaleY;
                    page.ForegroundContainer.RenderTransform = scaleTransform;
                    page.DecorationContainer.RenderTransform = scaleTransform;
                }
                else
                    page.RemoveWidgets();
            }
            UpdateCaret(page, false);
        }
        /// <summary>
        /// Creates the print preview pages.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PaginateEventArgs"/> instance containing the event data.</param>
        private void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            PrintDocument printDocument = sender as PrintDocument;
            // Report the number of preview pages created
            printDocument.SetPreviewPageCount(Pages.Count, PreviewPageCountType.Intermediate);
        }
#endif
        #endregion

        #region Implementations
        /// <summary>
        /// Updates the caret.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="removeCaret">if set to <c>true</c> remove Caret.</param>
        internal void UpdateCaret(PageAdv page, bool removeCaret)
        {
            if (removeCaret)
            {
                if (SelectionEndPage == page)
                {
                    SelectionEndPage.ForegroundContainer.Children.Remove(Caret);
                    SelectionEndPage.ForegroundContainer.Children.Remove(TouchEnd);
                }
                if (SelectionStartPage == page)
                    SelectionStartPage.ForegroundContainer.Children.Remove(TouchStart);
            }
            else
            {
                if (SelectionEndPage == page)
                {
                    if (!SelectionEndPage.ForegroundContainer.Children.Contains(Caret))
                        SelectionEndPage.ForegroundContainer.Children.Add(Caret);
                    if (!SelectionEndPage.ForegroundContainer.Children.Contains(TouchEnd))
                        SelectionEndPage.ForegroundContainer.Children.Add(TouchEnd);
                }
                if (SelectionStartPage == page
                    && !SelectionStartPage.ForegroundContainer.Children.Contains(TouchStart))
                    SelectionStartPage.ForegroundContainer.Children.Add(TouchStart);
            }
        }
        /// <summary>
        /// Inserts the page.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="page">The page.</param>
        internal void InsertPage(int index, PageAdv page)
        {
            Pages.Remove(page);
            Pages.Insert(index, page);
            double top = 20;
            if (index > 0)
                top += Pages[index - 1].BoundingRectangle.Bottom;
            for (int i = index; i < Pages.Count; i++)
            {
                //Update bounding rectangle of next pages in collection.
                page = Pages[i];
                page.BoundingRectangle = new Rect(page.BoundingRectangle.X, top, page.BoundingRectangle.Width, page.BoundingRectangle.Height);
                top = page.BoundingRectangle.Bottom + 20;
            }
        }
        /// <summary>
        /// Removes the page.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void RemovePage(PageAdv page)
        {
            if (CurrentPage == page)
                CurrentPage = null;
            if (SelectionStartPage == page)
                SelectionStartPage = null;
            if (SelectionEndPage == page)
                SelectionEndPage = null;
            if (Container != null)
                Container.Children.Remove(page);
            int index = Pages.IndexOf(page);
            Pages.Remove(page);
            if (this is PageLayoutViewer)
            {
                if ((this as PageLayoutViewer).VisiblePages != null)
                    (this as PageLayoutViewer).VisiblePages.Remove(page);
                //Updates the vertical height.
                VerticalHeight -= page.BoundingRectangle.Height + 20;
                //ToDo:Update horizontal width, if removed page has max width.
                double top = 20;
                if (index > 0)
                    top += Pages[index - 1].BoundingRectangle.Bottom;
                for (int i = index; i < Pages.Count; i++)
                {
                    //Update bounding rectangle of next pages in collection.
                    page = Pages[i];
                    page.BoundingRectangle = new Rect(page.BoundingRectangle.X, top, page.BoundingRectangle.Width, page.BoundingRectangle.Height);
                    top = page.BoundingRectangle.Bottom + 20;
                }
            }
            //Updates scroll bars max value.
            UpdateScrollBars();
        }
        /// <summary>
        /// Initializes the caret.
        /// </summary>
        private void InitCaret()
        {
            caret = new Rectangle();
            caret.Fill = new SolidColorBrush(Colors.Black);
            caret.Height = 12;
            caret.Width = 0.5;
            caret.Visibility = Visibility.Collapsed;
            Canvas.SetZIndex(caret, 4);
            storyBoard = new Storyboard();
            doubleAnimation = new DoubleAnimationUsingKeyFrames();
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation.AutoReverse = true;
            doubleAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 600));
            DiscreteDoubleKeyFrame keyFrame = new DiscreteDoubleKeyFrame();
            keyFrame.KeyTime = new TimeSpan(0, 0, 0, 0, 300);
            keyFrame.Value = 0;
            doubleAnimation.KeyFrames.Add(keyFrame);
            storyBoard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, caret);
#if WPF
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(UIElement.OpacityProperty));
#else
            Storyboard.SetTargetProperty(doubleAnimation, "Opacity");
#endif
            storyBoard.Begin();
            ScaleTransform scale = new ScaleTransform();
            scale.ScaleX = scale.ScaleY = 1;
            caret.RenderTransform = scale;

            //Initializes the touch selection start ellipse.
            touchStart = new Ellipse();
            touchStart.Height = 18;
            touchStart.Width = 18;
            touchStart.Visibility = Visibility.Collapsed;
            touchStart.Stroke = new SolidColorBrush(Colors.Black);
            touchStart.StrokeThickness = 1.5;
            touchStart.Fill = new SolidColorBrush(Colors.White);
            Canvas.SetZIndex(touchStart, 5);
            //Initializes the touch selection end ellipse.
            touchEnd = new Ellipse();
            touchEnd.Height = 18;
            touchEnd.Width = 18;
            touchEnd.Visibility = Visibility.Collapsed;
            touchEnd.Stroke = new SolidColorBrush(Colors.Black);
            touchEnd.StrokeThickness = 1.5;
            touchEnd.Fill = new SolidColorBrush(Colors.White);
            Canvas.SetZIndex(touchEnd, 6);
        }
        /// <summary>
        /// Resets the zooming.
        /// </summary>
        internal void ResetZooming()
        {
            ScaleFactor = 1;
            Zoom();
        }
        /// <summary>
        /// Resets the scroll bars.
        /// </summary>
        internal void ResetScrollBars()
        {
            if (HorizontalScrollBar != null)
            {
                HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                HorizontalScrollBar.Value = 0;
                HorizontalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                HorizontalScrollBar.Visibility = Visibility.Collapsed;
            }
            if (VerticalScrollBar != null)
            {
                VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                VerticalScrollBar.Value = 0;
                VerticalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                VerticalScrollBar.Visibility = Visibility.Collapsed;
            }
            Visiblebounds = new Rect(0, 0, Visiblebounds.Width, Visiblebounds.Height);
            transform.X = transform.Y = 0;
        }
        /// <summary>
        /// Updates the page background.
        /// </summary>
        internal void UpdatePageBackground(Color background)
        {
            foreach (PageAdv page in Pages)
            {
                page.Background = new SolidColorBrush(background);
                if (this is PageLayoutViewer && (this as PageLayoutViewer).VisiblePages.Contains(page))
                {
                    page.RemoveWidgets();
                    page.RenderWidgets();
                }
                else if (this is FlowLayoutViewer && OwnerControl.LayoutType == LayoutType.Continuous)
                {
                    page.RemoveWidgets();
                    page.RenderWidgets(this as FlowLayoutViewer);
                }
            }
        }
        /// <summary>
        /// Updates the current page number.
        /// </summary>
        internal void UpdateCurrentPageNumber()
        {
            if (this != null && this.Pages !=null)
            {
                for (int p = 0; p < this.Pages.Count; p++)
                {
                    PageAdv page = this.Pages[p];
                    if (page != null && this.CurrentPage != null && page == this.CurrentPage)
                    {
                        OwnerControl.CurrentPageNumber = p + 1;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Checks for cursor visibility.
        /// </summary>
        /// <param name="isTouch">if set to <c>true</c> [is touch].</param>
        internal void CheckForCursorVisibility(bool isTouch)
        {
            //Enables/Disables Caret based on the selection.
#if WPF
            if (OwnerControl.IsFocused)
#else
            if (OwnerControl.FocusState == FocusState.Unfocused)
#endif
                HideCaret(isTouch);
            else
                ShowCaret(isTouch);
        }
        /// <summary>
        /// Clears the container.
        /// </summary>
        internal void ClearContainer()
        {
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (VerticalScrollBar != null)
                {
                    VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                    VerticalScrollBar.ClearValue(ScrollBar.MaximumProperty);
                    VerticalScrollBar.ClearValue(ScrollBar.MinimumProperty);
                    VerticalScrollBar.ClearValue(ScrollBar.LargeChangeProperty);
                    VerticalScrollBar.ClearValue(ScrollBar.SmallChangeProperty);
                    VerticalScrollBar.ClearValue(ScrollBar.ValueProperty);
                }
                if (HorizontalScrollBar != null)
                {
                    HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                    HorizontalScrollBar.ClearValue(ScrollBar.MaximumProperty);
                    HorizontalScrollBar.ClearValue(ScrollBar.MinimumProperty);
                    HorizontalScrollBar.ClearValue(ScrollBar.LargeChangeProperty);
                    HorizontalScrollBar.ClearValue(ScrollBar.SmallChangeProperty);
                    HorizontalScrollBar.ClearValue(ScrollBar.ValueProperty);
                }
                if (this is PageLayoutViewer)
                    (this as PageLayoutViewer).ClearVisiblePages();
                Container.Children.Clear();
                RemovePages();
                Container.ClearValue(RenderTransformProperty);
                HideImageResizer();
                Visiblebounds = new Rect(0, 0, Visiblebounds.Width, Visiblebounds.Height);
                HorizontalWidth = 0;
                VerticalHeight = 0;
#if !WPF
            });
#endif
        }
        /// <summary>
        /// Removes all the pages from the container
        /// </summary>
        private void RemovePages()
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                PageAdv page = Pages[i];
                if (Container != null)
                    Container.Children.Remove(page);
                page.Dispose();
                Pages.Remove(page);
                i--;
            }
            if (CurrentPage != null)
            {
                CurrentPage.Dispose();
                CurrentPage.Viewer = null;
                CurrentPage = null;
            }
            if (SelectionStartPage != null)
                SelectionStartPage = null;
            if (SelectionEndPage != null)
                SelectionEndPage = null;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal virtual void Dispose()
        {
#if !WPF
            if (isPrinterRegistered)
                UnRegisterPrinting();
            DisposePrintDocument();
#endif
            if (VerticalScrollBar != null)
                VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
            if (HorizontalScrollBar != null)
                HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
            OwnerControl = null;
            transform = null;
            Children.Clear();
            RemovePages();
            if (Container != null)
            {
                Container.Children.Clear();
                Container = null;
            }
            ImageResizer.Dispose();
            ImageResizer = null;
            caret = null;
            storyBoard.Stop();
            doubleAnimation.KeyFrames.Clear();
            doubleAnimation = null;
            storyBoard.Children.Clear();
            storyBoard = null;
        }
        #endregion
    }
}