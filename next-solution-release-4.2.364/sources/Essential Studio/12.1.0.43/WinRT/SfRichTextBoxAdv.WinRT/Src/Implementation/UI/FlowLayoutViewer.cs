#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Threading;
#if WPF
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using RangeBaseValueChangedEventHandlerInternal = System.Windows.RoutedPropertyChangedEventHandler<double>;
using RangeBaseValueChangedEventArgsInternal = System.Windows.RoutedPropertyChangedEventArgs<double>;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using RangeBaseValueChangedEventHandlerInternal = Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventHandler;
using RangeBaseValueChangedEventArgsInternal = Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Core;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class FlowLayoutViewer : LayoutViewer
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FlowLayoutViewer"/> class.
        /// </summary>
        /// <param name="richTextBox"></param>
        internal FlowLayoutViewer(SfRichTextBoxAdv richTextBox)
            : base(richTextBox)
        {
#if WPF && SyncfusionFramework4_0
            this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(FlowLayoutViewer_ManipulationDelta);
            this.IsManipulationEnabled = richTextBox.IsManipulationEnabled;
#endif
            Padding = new Thickness(10);
            InitPage();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Initializes the page.
        /// </summary>
        private void InitPage()
        {
            CurrentPage = new PageAdv();
            CurrentPage.SetBackground(OwnerControl);
            Pages.Add(CurrentPage);
            CurrentPage.Viewer = this;
            Container.Children.Add(CurrentPage);
        }
        /// <summary>
        /// Gets the height of the content.
        /// </summary>
        /// <returns></returns>
        internal double GetContentHeight()
        {
            double height = 0;
            if (CurrentPage != null && CurrentPage.BodyWidgets.Count > 0)
                height = CurrentPage.BodyWidgets[0].Height;
            return height;
        }
        /// <summary>
        /// Updates the vertical scroll bar
        /// </summary>
        /// <param name="height">The height.</param>
        private void UpdateVerticalScrollBar(double height)
        {
            if (VerticalScrollBar == null)
                return;
            if (height > Visiblebounds.Height && OwnerControl.LayoutType == LayoutType.Continuous)
            {
                VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                if (OwnerControl.VerticalScrollBarVisibility)
                    VerticalScrollBar.Visibility = Visibility.Visible;
                VerticalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                VerticalScrollBar.ViewportSize = Visiblebounds.Height;
                VerticalScrollBar.Maximum = height - Visiblebounds.Height;
                VerticalScrollBar.Minimum = 0;
                VerticalScrollBar.SmallChange = 30;
                VerticalScrollBar.LargeChange = Visiblebounds.Height;
            }
            else
            {
                VerticalScrollBar.Maximum = 0;
                VerticalScrollBar.Visibility = Visibility.Collapsed;
                VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                transform.Y = 0;
            }
        }
        /// <summary>
        /// Updates the horizontal scroll bar.
        /// </summary>
        /// <param name="width">The width.</param>
        private void UpdateHorizontalScrollBar(double width)
        {
            if (HorizontalScrollBar == null)
                return;
            if (width > Visiblebounds.Width && OwnerControl.LayoutType == LayoutType.Continuous)
            {
                HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                if (OwnerControl.HorizontalScrollBarVisibility)
                    HorizontalScrollBar.Visibility = Visibility.Visible;
                HorizontalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                HorizontalScrollBar.ViewportSize = Visiblebounds.Width;
                HorizontalScrollBar.Maximum = width - Visiblebounds.Width;
                HorizontalScrollBar.Minimum = 0;
                HorizontalScrollBar.SmallChange = 30;
                HorizontalScrollBar.LargeChange = Visiblebounds.Width;
            }
            else
            {
                HorizontalScrollBar.Maximum = 0;
                HorizontalScrollBar.Visibility = Visibility.Collapsed;
                HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                transform.X = 0;
            }
        }
        /// <summary>
        /// Sets the render transforms.
        /// </summary>
        private void SetRenderTransforms()
        {
            Container.RenderTransform = transform;
            if (CurrentPage != null)
            {
                CurrentPage.ForegroundContainer.RenderTransform = scaleTransform;
                CurrentPage.DecorationContainer.RenderTransform = scaleTransform;
            }
        }
        /// <summary>
        /// Updates the cursor.
        /// </summary>
        /// <param name="point">The point.</param>
        private void UpdateCursor(Point point)
        {
            double leftMargin = ClientArea.X * ScaleFactor;
#if WPF
#else
            if ((OwnerControl.Selection.IsEmpty || ImageResizer.Visibility == Visibility.Visible)
                && point.X >= leftMargin)
                OwnerControl.Cursor = new CoreCursor(CoreCursorType.IBeam, 0);
            else
                OwnerControl.Cursor = new CoreCursor(CoreCursorType.Arrow, 0);
#endif
        }
#if WPF && SyncfusionFramework4_0
        /// <summary>
        /// Handles the ManipulationDelta event of the FlowLayoutViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ManipulationDeltaEventArgs" /> instance containing the event data.</param>
        void FlowLayoutViewer_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (isTouchDownOnSelectionMark)
            {
                base.OnManipulationDelta(e);
                return;
            }
            if (e.DeltaManipulation.Scale.Length == 1.4142135623730951
                || (e.DeltaManipulation.Scale.X == 1d && e.DeltaManipulation.Scale.Y == 1d))
            {
                base.OnManipulationDelta(e);
            }
            else
            {
                //Handled for zooming the page view size.
                e.Handled = true;
                if (e.DeltaManipulation.Scale.Length < 1.4142135623730951)
                {
                    if (ScaleFactor >= 0.25)
                        ScaleFactor -= 0.05;
                }
                else if (e.DeltaManipulation.Scale.Length > 1.4142135623730951)
                {
                    if (ScaleFactor <= 4)
                        ScaleFactor += 0.05;
                }
                OwnerControl.m_zoomFlag = true;
                OwnerControl.ZoomFactor = ScaleFactor / 4;
                Zoom();
            }
            //Handled for panning (Move) the page to view particular region.
            if (e.DeltaManipulation.Translation.X != 0)
                this.HorizontalScrollBar.Value = HorizontalScrollBar.Value - e.DeltaManipulation.Translation.X;
            if (e.DeltaManipulation.Translation.Y != 0)
                this.VerticalScrollBar.Value = VerticalScrollBar.Value - e.DeltaManipulation.Translation.Y;
        }
#endif
        #endregion

        #region Override Methods
        /// <summary>
        /// Creates the new page.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        internal override PageAdv CreateNewPage(SectionAdv section)
        {
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (Pages.Count == 0)
                    InitPage();
                UpdateClientArea();
#if !WPF
            });
#endif
            if (CurrentPage.BodyWidgets.Count == 0)
                //Adds new body widget for the content layouted in new page.
                CurrentPage.BodyWidgets.Add(section.AddBodyWidget(ClientActiveArea));
            return CurrentPage;
        }
        /// <summary>
        /// Adds the empty page.
        /// </summary>
        internal override void AddEmptyPage()
        {
            PageAdv emptyPage = new PageAdv();
            emptyPage.Width = Visiblebounds.Width;
            emptyPage.Height = Visiblebounds.Height;
            emptyPage.SetBackground(OwnerControl);
            //Adds an empty page in viewer, to preserve empty page till the content rendered.
            Container.Children.Add(emptyPage);
        }
        /// <summary>
        /// Determines whether loading preload pages.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if loading preload pages; otherwise, <c>false</c>.
        /// </returns>
        internal override bool IsLoadingPreloadPages()
        {
            return !OwnerControl.IsDocumentLoaded;
        }
        /// <summary>
        /// Updates the page bounds and scrollbar.
        /// </summary>
        internal override void UpdateScrollBars()
        {
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                double width = (HorizontalWidth * ScaleFactor > Visiblebounds.Width) ? HorizontalWidth * ScaleFactor : Visiblebounds.Width;
                double height = GetContentHeight() * ScaleFactor + Padding.Top + Padding.Bottom;
                if (height < Visiblebounds.Height)
                    height = Visiblebounds.Height;
                //Updates horizontal scroll bar values.
                UpdateHorizontalScrollBar(width);
                //Updates vertical scroll bar values.
                UpdateVerticalScrollBar(height);
                if (VerticalScrollBar != null && VerticalScrollBar.Visibility == Visibility.Collapsed
                    && OwnerControl.LayoutType != LayoutType.Block)
                    width = Visiblebounds.Width + Math.Max(VerticalScrollBar.Width, VerticalScrollBar.MinWidth);
                Container.Width = width;
                Container.Height = height;
                if (CurrentPage != null)
                {
                    Container.Children.Remove(CurrentPage);
                    CurrentPage.Width = width;
                    CurrentPage.Height = height;
                    CurrentPage.BoundingRectangle = new Rect(0, 0, width, height);
                    Container.Children.Add(CurrentPage);
                }
                SetRenderTransforms();
#if !WPF
            });
#endif
        }
        /// <summary>
        /// Handles the horizontal scroll bar value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        internal override void HorizontalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgsInternal e)
        {
            if (!OwnerControl.IsDocumentLoaded || OwnerControl.LayoutType == LayoutType.Block)
            {
                HorizontalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                HorizontalScrollBar.Value = e.OldValue;
                HorizontalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(HorizontalScrollBar_ValueChanged);
                return;
            }
            transform.X = -HorizontalScrollBar.Value;
            if (Visiblebounds.X != e.NewValue)
                Visiblebounds = new Rect(e.NewValue, Visiblebounds.Y, Visiblebounds.Width, Visiblebounds.Height);
            SetRenderTransforms();
        }
        /// <summary>
        /// Handles the vertical scroll bar value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        internal override void VerticalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgsInternal e)
        {
            if (!OwnerControl.IsDocumentLoaded || OwnerControl.LayoutType == LayoutType.Block)
            {
                VerticalScrollBar.ValueChanged -= new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                VerticalScrollBar.Value = e.OldValue;
                VerticalScrollBar.ValueChanged += new RangeBaseValueChangedEventHandlerInternal(VerticalScrollBar_ValueChanged);
                return;
            }
            transform.Y = -VerticalScrollBar.Value;
            if (Visiblebounds.Y != e.NewValue)
                Visiblebounds = new Rect(Visiblebounds.X, e.NewValue, Visiblebounds.Width, Visiblebounds.Height);
            RenderVisiblePages();
            SetRenderTransforms();
        }
         /// <summary>
        /// Renders the visible pages.
        /// </summary>
        internal override void RenderVisiblePages()
        {
            if (CurrentPage != null)
            {
                CurrentPage.RemoveWidgets();
                CurrentPage.RenderWidgets(this);
            }
        }
        /// <summary>
        /// Zooms this instance.
        /// </summary>
        internal override void Zoom()
        {
            if (ScaleFactor == scaleTransform.ScaleY)
                return;
            double prevScaleFactor = scaleTransform.ScaleY;
            scaleTransform.ScaleX = scaleTransform.ScaleY = ScaleFactor;
            if (TouchStart != null)
            {
                TouchStart.Width = 18.0 / ScaleFactor;
                TouchStart.Height = 18.0 / ScaleFactor;
                TouchStart.StrokeThickness = 1.5 / ScaleFactor;
            }
            if (TouchEnd != null)
            {
                TouchEnd.Width = 18.0 / ScaleFactor;
                TouchEnd.Height = 18.0 / ScaleFactor;
                TouchEnd.StrokeThickness = 1.5 / ScaleFactor;
            }
            if (VerticalScrollBar != null && HorizontalScrollBar != null)
            {
                double xValue = HorizontalScrollBar.Value;
                double yValue = VerticalScrollBar.Value;
                //Relayouts the content in the modified layout area, based on new scale factor.
                OwnerControl.Document.LayoutItems();
                //Updates the horizontal scrollbar based on previous value and scale factor.
                HorizontalScrollBar.Value = xValue * ScaleFactor / prevScaleFactor;
                //Updates the vertical scrollbar based on previous value and scale factor.
                VerticalScrollBar.Value = yValue * ScaleFactor / prevScaleFactor;
            }
        }
        /// <summary>
        /// Finds the focused page
        /// </summary>
        /// <param name="e"></param>
        internal override void FindFocusedPage(MouseEventArgs e)
        {
            if (Pages.Count == 0)
                return;
            CurrentPage = Pages[0];
        }
#if !WPF
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(PointerRoutedEventArgs e)
        {
            if (Pages.Count == 0)
                return;
            CurrentPage = Pages[0];
            Point point = e.GetCurrentPoint(Container).Position;
            UpdateCursor(point);
        }
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="TappedRoutedEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(TappedRoutedEventArgs e)
        {
            if (Pages.Count == 0)
                return;
            CurrentPage = Pages[0];
            Point point = e.GetPosition(Container);
            UpdateCursor(point);
        }
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="DoubleTappedRoutedEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(DoubleTappedRoutedEventArgs e)
        {
            if (Pages.Count == 0)
                return;
            CurrentPage = Pages[0];
            Point point = e.GetPosition(Container);
            UpdateCursor(point);
        }
        /// <summary>
        /// Updates the core cursor.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        internal override void UpdateCoreCursor(PointerRoutedEventArgs e)
        {
            if (Pages.Count == 0)
                return;
            Point point = e.GetCurrentPoint(Container).Position;
            double leftMargin = ClientArea.X * ScaleFactor;
            PointerPoint pointerPoint = e.GetCurrentPoint(CurrentPage.ForegroundContainer);
            LineWidget currentLineWidget = GetLineWidget(pointerPoint.Position);
            FieldBeginAdv hyperlinkField = null;
            if (currentLineWidget != null)
                hyperlinkField = currentLineWidget.GetHyperlinkField(OwnerControl, pointerPoint.Position);
#if WPF
#else
            if (hyperlinkField != null && pointerPoint.PointerDevice.PointerDeviceType == PointerDeviceType.Mouse
                && (OwnerControl.ModifierKey & VirtualKeyModifiers.Control) == VirtualKeyModifiers.Control)
                OwnerControl.Cursor = new CoreCursor(CoreCursorType.Hand, 0);
            else if ((OwnerControl.Selection.IsEmpty || ImageResizer.Visibility == Visibility.Visible)
                && point.X >= leftMargin)
                OwnerControl.Cursor = new CoreCursor(CoreCursorType.IBeam, 0);
            else
                OwnerControl.Cursor = new CoreCursor(CoreCursorType.Arrow, 0);
#endif
        }
#endif
#if WPF && SyncfusionFramework4_0
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        internal override void FindFocusedPage(TouchEventArgs e)
        {
            if (Pages.Count == 0)
                return;
            CurrentPage = Pages[0];
        }
#endif
        /// <summary>
        /// Releases the resources
        /// </summary>
        internal override void Dispose()
        {
#if WPF && SyncfusionFramework4_0
            this.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(FlowLayoutViewer_ManipulationDelta);
#endif
            base.Dispose();
        }
        #endregion
    }
}
