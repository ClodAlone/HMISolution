// <copyright file="DocumentContainerHelper.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using Syncfusion.Windows.Shared;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Helper class, used to setup internal parts of the document container and process some of the input.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class DocumentContainerHelper : DependencyObject
    {
        #region Dependency properties
        /// <summary>
        /// Attached dependency property that indicates whether the specified element is the MDI border.
        /// </summary>
        public static readonly DependencyProperty IsMDIBorderProperty = DependencyProperty.RegisterAttached("IsMDIBorder", typeof(bool), typeof(DocumentContainerHelper), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsMDIBorderChanged)));
        
        /// <summary>
        /// Attached dependency property that indicates whether the specified element is the MDI header.
        /// </summary>
        public static readonly DependencyProperty IsMDIHeaderProperty = DependencyProperty.RegisterAttached("IsMDIHeader", typeof(bool), typeof(DocumentContainerHelper), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsMDIHeaderChanged)));
        
        /// <summary>
        /// Specifies whether the IsActive property of the document should be forced to be set to true instead of depending from the focus.
        /// </summary>
        public static readonly DependencyProperty ForceIsActiveProperty = DependencyProperty.RegisterAttached("ForceIsActive", typeof(bool), typeof(DocumentContainerHelper), new FrameworkPropertyMetadata(false));
        
        /// <summary>
        /// Represents the HoldSizeProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty HoldSizeProperty = DependencyProperty.RegisterAttached("HoldSize", typeof(Size), typeof(DocumentContainerHelper), new FrameworkPropertyMetadata(Size.Empty, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region DP Getters and Setters
        /// <summary>
        /// Gets value, indicating whether the specified element is set to be the MDI border.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns> bool IsMDIBorderProperty</returns>
        public static bool GetIsMDIBorder(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMDIBorderProperty);
        }

        /// <summary>
        /// Sets value, indicating whether the specified element is set to be the MDI border.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsMDIBorder(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMDIBorderProperty, value);
        }

        /// <summary>
        /// Gets value, indicating whether the specified element is set to be the MDI header.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>bool IsMDIHeaderProperty</returns>
        public static bool GetIsMDIHeader(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMDIHeaderProperty);
        }

        /// <summary>
        /// Sets value, indicating whether the specified element is set to be the MDI header.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsMDIHeader(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMDIHeaderProperty, value);
        }
        
        /// <summary>
        /// Gets the force is active.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns> bool ForceIsActiveProperty</returns>
        public static bool GetForceIsActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(ForceIsActiveProperty);
        }
        
        /// <summary>
        /// Sets the force is active.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetForceIsActive(DependencyObject obj, bool value)
        {
            obj.SetValue(ForceIsActiveProperty, value);
        }
        
        /// <summary>
        /// Gets the size of the hold.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>Size HoldSizeProperty</returns>
        public static Size GetHoldSize(DependencyObject obj)
        {
            return (Size)obj.GetValue(HoldSizeProperty);
        }
        
        /// <summary>
        /// Sets the size of the hold.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value.</param>
        public static void SetHoldSize(DependencyObject obj, Size value)
        {
            obj.SetValue(HoldSizeProperty, value);
        }
        #endregion

        #region DP Change handlers
        /// <summary>
        /// Processes the change of the IsMDIBorder attached dependency property on an object.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsMDIBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool value = (bool)e.NewValue;
            UIElement element = d as UIElement;

            if (element == null)
            {
                throw new InvalidOperationException("IsMDIBorder property can be set only on UIElements");
            }

            if (value)
            {
                element.AddHandler(Mouse.QueryCursorEvent, new QueryCursorEventHandler(GetCursorOnBorder));
            }
            else
            {
                element.RemoveHandler(Mouse.QueryCursorEvent, new QueryCursorEventHandler(GetCursorOnBorder));
            }

            PrepareMouseDownEvent(value, !GetIsMDIHeader(element), element);
            #if !SyncfusionFramework3_5
            //PrepareTouchDownEvent(value, !GetIsMDIHeader(element), element);
#endif
        }
        
        /// <summary>
        /// Processes the change of the IsMDIHeader attached dependency property on an object.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsMDIHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool value = (bool)e.NewValue;
            UIElement element = d as UIElement;

            if (element == null)
            {
                throw new InvalidOperationException("IsMDIBorder property can be set only on UIElements");
            }

            PrepareMouseDownEvent(value, !GetIsMDIBorder(element), element);
            #if !SyncfusionFramework3_5
            //PrepareTouchDownEvent(value, !GetIsMDIBorder(element), element);
#endif
        }
        
        /// <summary>
        /// Prepares the mouse down event.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="isntValue">if set to <c>true</c> [isnt value].</param>
        /// <param name="element">The element.</param>
        private static void PrepareMouseDownEvent(bool value, bool isntValue, IInputElement element)
        {
            if (isntValue)
            {
                if (value)
                {
                    element.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(ProcessMouseDown));
                }
                else
                {
                    element.RemoveHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(ProcessMouseDown));
                }
            }
        }
#if !SyncfusionFramework3_5
        /// <summary>
        /// Prepares the touch down event.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="isntValue">if set to <c>true</c> [isnt value].</param>
        /// <param name="element">The element.</param>
        //private static void PrepareTouchDownEvent(bool value, bool isntValue, IInputElement element)
        //{
        //    if (isntValue)
        //    {
        //        if (value)
        //        {
        //            element.AddHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(ProcessTouchDown));
        //        }
        //        else
        //        {
        //            element.RemoveHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(ProcessTouchDown));
        //        }
        //    }
        //}
#endif
        #endregion

        #region Implementation
        /// <summary>
        /// Gets cursor, used for the specified border side. Center, Outside and Header values are ignored and no cursor is returned.
        /// </summary>
        /// <param name="border">Border side to get cursor for.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>Cursor object.</returns>
        private static Cursor GetCursor(MDIBorder border, FlowDirection direction)
        {
            Cursor cursor = null;

            switch (border)
            {
                case MDIBorder.Right:
                case MDIBorder.Left:
                    cursor = Cursors.SizeWE;
                    break;

                case MDIBorder.RightBottom:
                case MDIBorder.LeftTop:
                    cursor = FlowDirection.LeftToRight == direction
                        ? Cursors.SizeNWSE : Cursors.SizeNESW;
                    break;

                case MDIBorder.Bottom:
                case MDIBorder.Top:
                    cursor = Cursors.SizeNS;
                    break;

                case MDIBorder.RightTop:
                case MDIBorder.LeftBottom:
                    cursor = FlowDirection.LeftToRight == direction
                        ? Cursors.SizeNESW : Cursors.SizeNWSE;
                    break;
            }

            return cursor;
        }
        
        /// <summary>
        /// Gets border side of the element, the specified point is atop of.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="point">The point.</param>
        /// <returns>MDIBorder element</returns>
        private static MDIBorder GetSide(UIElement element, Point point)
        {
            Size size = element.RenderSize;

            object valuePadding = element.GetValue(Border.PaddingProperty);
            object valueCornerRadius = element.GetValue(Border.CornerRadiusProperty);
            object valueThickness = element.GetValue(Border.BorderThicknessProperty);

            if (valueThickness == null || valueThickness == DependencyProperty.UnsetValue)
            {
                valueThickness = new Thickness(2);
            }

            Thickness thickness = (Thickness)valueThickness;

            if (valuePadding != null && valuePadding != DependencyProperty.UnsetValue)
            {
                Thickness padding = (Thickness)valuePadding;
                thickness.Left += padding.Left;
                thickness.Right += padding.Right;
                thickness.Top += padding.Top;
                thickness.Bottom += padding.Bottom;
            }

            double x = point.X;
            double y = point.Y;
            double bottom = size.Height - 1;
            double right = size.Width - 1;
            const double DefOffset = 3.0;

            if (x < 0 || x > right || y < 0 || y > bottom)
            {
                return MDIBorder.Outside;
            }

            double leftValue = thickness.Left > DefOffset ? thickness.Left : DefOffset;
            double topValue = thickness.Top > DefOffset ? thickness.Top : DefOffset;
            double rightValue = thickness.Right > DefOffset ? thickness.Right : DefOffset;
            double bottomValue = thickness.Bottom > DefOffset ? thickness.Bottom : DefOffset;

            if (x < leftValue)
            {
                if (y < topValue + ((CornerRadius)valueCornerRadius).TopLeft)
                {
                    return MDIBorder.LeftTop;
                }

                if (y > bottom - bottomValue - ((CornerRadius)valueCornerRadius).BottomLeft)
                {
                    return MDIBorder.LeftBottom;
                }

                return MDIBorder.Left;
            }

            if (x > right - rightValue)
            {
                if (y < topValue + ((CornerRadius)valueCornerRadius).TopRight)
                {
                    return MDIBorder.RightTop;
                }

                if (y > bottom - bottomValue - ((CornerRadius)valueCornerRadius).BottomRight)
                {
                    return MDIBorder.RightBottom;
                }

                return MDIBorder.Right;
            }

            if (y < topValue)
            {
                if (x < leftValue + ((CornerRadius)valueCornerRadius).TopLeft)
                {
                    return MDIBorder.LeftTop;
                }

                if (x > right - rightValue - ((CornerRadius)valueCornerRadius).TopRight)
                {
                    return MDIBorder.RightTop;
                }

                return MDIBorder.Top;
            }

            if (y > bottom - bottomValue)
            {
                if (x < leftValue + ((CornerRadius)valueCornerRadius).BottomLeft)
                {
                    return MDIBorder.LeftBottom;
                }

                if (x > right - rightValue - ((CornerRadius)valueCornerRadius).BottomRight)
                {
                    return MDIBorder.RightBottom;
                }

                return MDIBorder.Bottom;
            }

            return MDIBorder.Center;
        }

        /// <summary>
        /// Limits cursor movement to the bounds of the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        private static void LockCursorOnElement(UIElement element)
        {
            #if SyncfusionFramework3_5
            PresentationSource source = PresentationSource.FromVisual(element);
            
            Point pointScreenLeftTop = element.PointToScreen(new Point(1, 1));
            Point pointScreenRightBottom = element.PointToScreen(new Point(element.RenderSize.Width - 2, element.RenderSize.Height - 2));

            Matrix transform = source.CompositionTarget.TransformToDevice;
            pointScreenLeftTop = transform.Transform(pointScreenLeftTop);
            pointScreenRightBottom = transform.Transform(pointScreenRightBottom);

            RECT rect = new RECT(pointScreenLeftTop, pointScreenRightBottom);
            NativeMethods.ClipCursor(ref rect);
            #endif
        }

        /// <summary>
        /// Removes the limits of the cursors movements.
        /// </summary>
        private static void UnlockCursor()
        {
            #if SyncfusionFramework3_5
            NativeMethods.ClipCursor(null);
            #endif
        }
        
        /// <summary>
        /// Gets the percent point.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>PercentPoint panel</returns>
        private static PercentPoint GetPercentPoint(UIElement panel, InputEventArgs e)
        {
            Point point=new Point();
            if (e is MouseEventArgs)
                point = (e as MouseEventArgs).GetPosition(panel);
#if !SyncfusionFramework3_5
            else if (e is TouchEventArgs)
                point = (e as TouchEventArgs).GetTouchPoint(panel).Position;
#endif
            return GetPercentPoint(panel, point);
        }
        
        /// <summary>
        /// Gets the percent point.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="point">The point.</param>
        /// <returns>PercentPoint panel</returns>
        private static PercentPoint GetPercentPoint(UIElement panel, Point point)
        {
            Size size = panel.RenderSize;
            return new PercentPoint(point.X / size.Width, point.Y / size.Height);
        }
        
        /// <summary>
        /// Sets the bounds.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="content">The content.</param>
        /// <param name="newRect">The new rect.</param>
        /// <param name="isMinimized">if set to <c>true</c> [is minimized].</param>
        private static void SetBounds(MDILayoutPanel panel, DependencyObject content, Rect newRect, bool isMinimized)
        {
            if (HasChangedRect(content, newRect))
            {
                DocumentContainer owner = panel.Container;

                newRect = ValidateRect(owner.UseInteropCompatibility, newRect, owner.RenderSize);

                if (isMinimized)
                {                    
                    Style style = null;
                    style = (Style)DocumentContainer.GetDocumentMDIHeaderStyle(owner);

                    if (owner.IsInDockingManager)
                    {
                        style = (Style)DockingManager.GetDocumentMDIHeaderStyle(owner) ??
                           (Style)DockingManager.GetDocumentMDIHeaderStyle(owner.FlipParent as DockingManager);
                    }

                    double docheight = DocumentContainer.MINIMIZED_HEIGHT;
                    DocumentHeader docHeader = new DocumentHeader();                  
                    bool heightflag = false;
                    if (style != null)
                    {
                        docHeader.Style = style;
                        SetterBaseCollection collection = docHeader.Style.Setters;
                        foreach (Setter setter in collection)
                        {
                            if (setter.Property.Equals(DocumentHeader.HeightProperty))
                            {
                                heightflag = true;
                                docheight = (double)setter.Value;
                            }
                        }
                    }

                    if (!heightflag)
                    {
                        newRect.Height = DocumentContainer.MINIMIZED_HEIGHT;
                    }
                    else
                    {
                        newRect.Height = docheight;
                    }

                    newRect.Width = DocumentContainer.MINIMIZED_WIDTH;
                    DocumentContainer.SetMDIMinimizedBounds(content, newRect);
                }
                else
                {
                    DocumentContainer.SetMDIBounds(content, newRect);
                }

                DocumentContainerHelper.SetHoldSize(panel, panel.RenderSize);
                panel.InvalidateArrange();
            }
        }
        
        /// <summary>
        /// Converts to percent rect.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="pSize">Size of the p.</param>
        /// <returns>PercentRect window</returns>
        private static PercentRect ConvertToPercentRect(MDIWindow window, Size pSize)
        {
            double pWidth = pSize.Width;
            double pHeight = pSize.Height;

            Rect rect = window.GetMDIBounds();
            Size size = window.RenderSize;

            return new PercentRect(rect.X / pWidth, rect.Y / pHeight, size.Width / pWidth, size.Height / pHeight);
        }
        
        /// <summary>
        /// Converts from percent rect.
        /// </summary>
        /// <param name="rect">The rect DependencyObject.</param>
        /// <param name="size">The size DependencyObject.</param>
        /// <returns>Rect PercentWidth</returns>
        private static Rect ConvertFromPercentRect(PercentRect rect, Size size)
        {
            double pWidth = size.Width;
            double pHeight = size.Height;

            return new Rect(rect.PercentX * pWidth, rect.PercentY * pHeight, rect.PercentWidth * pWidth, rect.PercentHeight * pHeight);
        }
        #endregion

        #region Input Event Handlers

        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void ProcessMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                FrameworkElement element = (FrameworkElement)sender;
                MDIWindow mdiWindow = VisualUtils.FindAncestor(element, typeof(MDIWindow)) as MDIWindow;

                if (mdiWindow == null)
                {
                    throw new InvalidOperationException("Can not find MDIWindow");
                }

                MDILayoutPanel panel = VisualUtils.FindAncestor(mdiWindow, typeof(MDILayoutPanel)) as MDILayoutPanel;

                if (panel == null)
                {
                    return;
                }

                DependencyObject content = (DependencyObject)element;
                bool allowResize = panel.Container.IsAllowMDIResize
                    && DocumentContainer.GetAllowMDIResize(content);
                bool isBorder = allowResize && GetIsMDIBorder(element);
                bool isHeader = GetIsMDIHeader(element);

                if (!isBorder && !isHeader)
                {
                    return;
                }

                MDIBorder side = isHeader ? MDIBorder.Header : GetSide(element, e.GetPosition(mdiWindow));

                if (side == MDIBorder.Center || side == MDIBorder.Outside)
                {
                    e.Handled = true;
                    return;
                }

                PercentPoint realPoint = GetPercentPoint(panel, e);
                PercentRect rect = ConvertToPercentRect(mdiWindow, panel.RenderSize);
                MDIWindowDragInfo info = new MDIWindowDragInfo(rect, realPoint, side, mdiWindow);
                panel.DragWindowInfo = info;

                if (panel.CaptureMouse())
                {
                    panel.IsDragging = true;
                    panel.MouseMove += ProcessMouseMove;
                    panel.LostMouseCapture += ProcessLostMouseCapture;
                    panel.PreviewMouseUp += ProcessMouseUp;
                    panel.Cursor = GetCursor(side, panel.FlowDirection);

                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        //LockCursorOnElement((UIElement)panel.TemplatedParent);
                    }
                }

                e.Handled = true;
            }
        }
        
        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="arg">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void ProcessMouseUp(object sender, MouseButtonEventArgs arg)
        {
            if (arg.StylusDevice == null)
            {
                MDILayoutPanel panel = (MDILayoutPanel)sender;
                panel.ReleaseMouseCapture();
                panel.IsDragging = false;
            }
        }
        
        /// <summary>
        /// Processes the lost mouse capture.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private static void ProcessLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                MDILayoutPanel panel = (MDILayoutPanel)sender;
                panel.MouseMove -= ProcessMouseMove;
                panel.LostMouseCapture -= ProcessLostMouseCapture;
                panel.PreviewMouseUp -= ProcessMouseUp;
                panel.ClearValue(FrameworkElement.CursorProperty);
                DocumentContainerHelper.SetHoldSize(panel, Size.Empty);

                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    //UnlockCursor();
                }
            }
        }
        
        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private static void ProcessMouseMove(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                MDILayoutPanel panel = (MDILayoutPanel)sender;
                Point point = e.GetPosition(panel);

                if (panel != null && panel.DragWindowInfo.Content != null && DocumentContainer.GetCanDrag(panel.DragWindowInfo.Content))
                {
                    if (1 < point.X && 1 < point.Y)
                    {
                        MDIWindowDragInfo info = panel.DragWindowInfo;
                        PercentRect newRect = info.DragStartRect;
                        PercentPoint startPoint = info.DragStartPoint;

                        double x = startPoint.PercentX;
                        double y = startPoint.PercentY;

                        PercentPoint percentPoint = GetPercentPoint(panel, point);
                        double pointOffsetX = percentPoint.PercentX - x;
                        double pointOffsetY = percentPoint.PercentY - y;

                        double height = newRect.PercentHeight;
                        double width = newRect.PercentWidth;

                        MDIWindow window = info.WindowDragged;
                        Size size = panel.RenderSize;
                        double minWinWidth = window.MinWidth / size.Width;
                        double minWinHeight = window.MinHeight / size.Height;
                        double actualWinWidth = window.ActualWidth / size.Width;
                        double actualWinHeight = window.ActualHeight / size.Height;

                        bool isInMinWidth = (x + newRect.PercentWidth - minWinWidth) < percentPoint.PercentX;
                        bool isInMinHeight = (y + newRect.PercentHeight - minWinHeight) < percentPoint.PercentY;

                        int directFactor = FlowDirection.LeftToRight == panel.FlowDirection ? 1 : -1;

                        switch (info.Border)
                        {
                            case MDIBorder.Header:
                                ValidateMinimizedWindow(window);
                                newRect.PercentX += directFactor * pointOffsetX;
                                newRect.PercentY += pointOffsetY;
                                break;

                            case MDIBorder.Left:
                                newRect.PercentX += directFactor * ((isInMinWidth && pointOffsetX > 0)
                                    ? newRect.PercentWidth - minWinWidth : pointOffsetX);
                                width -= directFactor * pointOffsetX;
                                break;

                            case MDIBorder.LeftTop:
                                newRect.PercentX += directFactor * (isInMinWidth && pointOffsetX > 0
                                    ? newRect.PercentWidth - actualWinWidth
                                    : pointOffsetX);
                                width -= directFactor * pointOffsetX;
                                newRect.PercentY += (isInMinHeight && pointOffsetY > 0)
                                    ? newRect.PercentHeight - actualWinHeight
                                    : pointOffsetY;
                                height -= pointOffsetY;
                                break;

                            case MDIBorder.Top:
                                newRect.PercentY += (isInMinHeight && pointOffsetY > 0)
                                    ? newRect.PercentHeight - actualWinHeight
                                    : pointOffsetY;
                                height -= pointOffsetY;
                                break;

                            case MDIBorder.RightTop:
                                newRect.PercentY += (isInMinHeight && pointOffsetY > 0)
                                    ? newRect.PercentHeight - actualWinHeight
                                    : pointOffsetY;
                                height -= pointOffsetY;
                                width += directFactor * pointOffsetX;
                                break;

                            case MDIBorder.Right:
                                width += directFactor * pointOffsetX;
                                break;

                            case MDIBorder.RightBottom:
                                height += pointOffsetY;
                                width += directFactor * pointOffsetX;
                                break;

                            case MDIBorder.Bottom:
                                height += pointOffsetY;
                                break;

                            case MDIBorder.LeftBottom:
                                height += pointOffsetY;
                                newRect.PercentX += directFactor * (isInMinWidth && pointOffsetX > 0
                                    ? newRect.PercentWidth - actualWinWidth
                                    : pointOffsetX);
                                width -= directFactor * pointOffsetX;
                                break;

                            default:
                                break;
                        }

                        newRect.PercentWidth = GetSizeValue(minWinWidth, width);
                        newRect.PercentHeight = GetSizeValue(minWinHeight, height);

                        Rect rect = ConvertFromPercentRect(newRect, size);
                        if (window.Content == window.Container.ActiveDocument)
                        {
                            SetBounds(panel, info.Content, rect, window.IsMinimized);
                        }
                    }
                }
            }
        }

        #region TouchEvents
#if !SyncfusionFramework3_5

        //private static void ProcessTouchDown(object sender, TouchEventArgs e)
        //{
        //    FrameworkElement element = (FrameworkElement)sender;
        //    MDIWindow mdiWindow = VisualUtils.FindAncestor(element, typeof(MDIWindow)) as MDIWindow;

        //    if (mdiWindow == null)
        //    {
        //        throw new InvalidOperationException("Can not find MDIWindow");
        //    }

        //    MDILayoutPanel panel = VisualUtils.FindAncestor(mdiWindow, typeof(MDILayoutPanel)) as MDILayoutPanel;

        //    if (panel == null)
        //    {
        //        return;
        //    }
        //    DocumentContainer container = (panel != null) ? panel.Container : (mdiWindow != null) ? mdiWindow.Container : null;
        //    if (container != null && container.IsTouchEnabled && container.m_documentContainerTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        DependencyObject content = (DependencyObject)element;
        //        bool allowResize = panel.Container.IsAllowMDIResize
        //            && DocumentContainer.GetAllowMDIResize(content);
        //        bool isBorder = allowResize && GetIsMDIBorder(element);
        //        bool isHeader = GetIsMDIHeader(element);

        //        if (!isBorder && !isHeader)
        //        {
        //            return;
        //        }

        //        MDIBorder side = isHeader ? MDIBorder.Header : GetSide(element, e.GetTouchPoint(mdiWindow).Position);

        //        if (side == MDIBorder.Center || side == MDIBorder.Outside)
        //        {
        //            e.Handled = true;
        //            return;
        //        }

        //        PercentPoint realPoint = GetPercentPoint(panel, e);
        //        PercentRect rect = ConvertToPercentRect(mdiWindow, panel.RenderSize);
        //        MDIWindowDragInfo info = new MDIWindowDragInfo(rect, realPoint, side, mdiWindow);
        //        panel.DragWindowInfo = info;

        //        if (panel.CaptureTouch(e.TouchDevice))
        //        {
        //            panel.IsDragging = true;
        //            panel.TouchMove += ProcessTouchMove;
        //            panel.LostTouchCapture += ProcessLostTouchCapture;
        //            panel.PreviewTouchUp += ProcessPreviewTouchUp;

        //            panel.Cursor = GetCursor(side, panel.FlowDirection);

        //            if (!BrowserInteropHelper.IsBrowserHosted)
        //            {
        //                //LockCursorOnElement((UIElement)panel.TemplatedParent);
        //            }
        //        }

        //        e.Handled = true;
        //    }
        //}

        //static void ProcessPreviewTouchUp(object sender, TouchEventArgs e)
        //{
        //    MDILayoutPanel panel = (MDILayoutPanel)sender;
        //    panel.ReleaseMouseCapture();
        //    panel.IsDragging = false;
        //}

        //static void ProcessLostTouchCapture(object sender, TouchEventArgs e)
        //{
        //    MDILayoutPanel panel = (MDILayoutPanel)sender;
        //    panel.TouchMove -= ProcessTouchMove;
        //    panel.LostTouchCapture -= ProcessLostTouchCapture;
        //    panel.PreviewTouchUp -= ProcessPreviewTouchUp;
        //    panel.ClearValue(FrameworkElement.CursorProperty);
        //    DocumentContainerHelper.SetHoldSize(panel, Size.Empty);

        //    if (!BrowserInteropHelper.IsBrowserHosted)
        //    {
        //        //UnlockCursor();
        //    }
        //}

        //static void ProcessTouchMove(object sender, TouchEventArgs e)
        //{
        //    MDILayoutPanel panel = (MDILayoutPanel)sender;
        //    Point point = e.GetTouchPoint(panel).Position;

        //    if (panel != null && panel.DragWindowInfo.Content != null && DocumentContainer.GetCanDrag(panel.DragWindowInfo.Content))
        //    {
        //        if (1 < point.X && 1 < point.Y)
        //        {
        //            MDIWindowDragInfo info = panel.DragWindowInfo;
        //            PercentRect newRect = info.DragStartRect;
        //            PercentPoint startPoint = info.DragStartPoint;

        //            double x = startPoint.PercentX;
        //            double y = startPoint.PercentY;

        //            PercentPoint percentPoint = GetPercentPoint(panel, point);
        //            double pointOffsetX = percentPoint.PercentX - x;
        //            double pointOffsetY = percentPoint.PercentY - y;

        //            double height = newRect.PercentHeight;
        //            double width = newRect.PercentWidth;

        //            MDIWindow window = info.WindowDragged;
        //            Size size = panel.RenderSize;
        //            double minWinWidth = window.MinWidth / size.Width;
        //            double minWinHeight = window.MinHeight / size.Height;
        //            double actualWinWidth = window.ActualWidth / size.Width;
        //            double actualWinHeight = window.ActualHeight / size.Height;

        //            bool isInMinWidth = (x + newRect.PercentWidth - minWinWidth) < percentPoint.PercentX;
        //            bool isInMinHeight = (y + newRect.PercentHeight - minWinHeight) < percentPoint.PercentY;

        //            int directFactor = FlowDirection.LeftToRight == panel.FlowDirection ? 1 : -1;

        //            switch (info.Border)
        //            {
        //                case MDIBorder.Header:
        //                    ValidateMinimizedWindow(window);
        //                    newRect.PercentX += directFactor * pointOffsetX;
        //                    newRect.PercentY += pointOffsetY;
        //                    break;

        //                case MDIBorder.Left:
        //                    newRect.PercentX += directFactor * ((isInMinWidth && pointOffsetX > 0)
        //                        ? newRect.PercentWidth - minWinWidth : pointOffsetX);
        //                    width -= directFactor * pointOffsetX;
        //                    break;

        //                case MDIBorder.LeftTop:
        //                    newRect.PercentX += directFactor * (isInMinWidth && pointOffsetX > 0
        //                        ? newRect.PercentWidth - actualWinWidth
        //                        : pointOffsetX);
        //                    width -= directFactor * pointOffsetX;
        //                    newRect.PercentY += (isInMinHeight && pointOffsetY > 0)
        //                        ? newRect.PercentHeight - actualWinHeight
        //                        : pointOffsetY;
        //                    height -= pointOffsetY;
        //                    break;

        //                case MDIBorder.Top:
        //                    newRect.PercentY += (isInMinHeight && pointOffsetY > 0)
        //                        ? newRect.PercentHeight - actualWinHeight
        //                        : pointOffsetY;
        //                    height -= pointOffsetY;
        //                    break;

        //                case MDIBorder.RightTop:
        //                    newRect.PercentY += (isInMinHeight && pointOffsetY > 0)
        //                        ? newRect.PercentHeight - actualWinHeight
        //                        : pointOffsetY;
        //                    height -= pointOffsetY;
        //                    width += directFactor * pointOffsetX;
        //                    break;

        //                case MDIBorder.Right:
        //                    width += directFactor * pointOffsetX;
        //                    break;

        //                case MDIBorder.RightBottom:
        //                    height += pointOffsetY;
        //                    width += directFactor * pointOffsetX;
        //                    break;

        //                case MDIBorder.Bottom:
        //                    height += pointOffsetY;
        //                    break;

        //                case MDIBorder.LeftBottom:
        //                    height += pointOffsetY;
        //                    newRect.PercentX += directFactor * (isInMinWidth && pointOffsetX > 0
        //                        ? newRect.PercentWidth - actualWinWidth
        //                        : pointOffsetX);
        //                    width -= directFactor * pointOffsetX;
        //                    break;

        //                default:
        //                    break;
        //            }

        //            newRect.PercentWidth = GetSizeValue(minWinWidth, width);
        //            newRect.PercentHeight = GetSizeValue(minWinHeight, height);

        //            Rect rect = ConvertFromPercentRect(newRect, size);
        //            if (window.Content == window.Container.ActiveDocument)
        //            {
        //                SetBounds(panel, info.Content, rect, window.IsMinimized);
        //            }
        //        }
        //    }
        //}

#endif
        #endregion
        /// <summary>
        /// Validates the rect.
        /// </summary>
        /// <param name="useInteropCompatibility">if set to <c>true</c> [use interop compatibility].</param>
        /// <param name="rect">The rect DependencyObject.</param>
        /// <param name="parentSize">Size of the parent.</param>
        /// <returns>Rect rect value</returns>
        private static Rect ValidateRect(bool useInteropCompatibility, Rect rect, Size parentSize)
        {
            if (useInteropCompatibility)
            {
                if (rect.X < 0)
                {
                    rect.X = 0;
                }

                if (rect.Y < 0)
                {
                    rect.Y = 0;
                }

                if (rect.X + rect.Width > parentSize.Width)
                {
                    double offset = parentSize.Width - rect.X - rect.Width;
                    rect.X += offset;
                }

                if (rect.Y + rect.Height >= parentSize.Height)
                {
                    double offset = parentSize.Height - rect.Y - rect.Height;
                    rect.Y += offset;
                }
            }

            return rect;
        }
        
        /// <summary>
        /// Determines whether [has changed rect] [the specified content].
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="newRect">The new rect.</param>
        /// <returns>
        /// <c>true</c> if [has changed rect] [the specified content]; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasChangedRect(DependencyObject content, Rect newRect)
        {
            return newRect != (Rect)content.GetValue(DocumentContainer.MDIBoundsProperty);
        }
        
        /// <summary>
        /// Validates the minimized window.
        /// </summary>
        /// <param name="window">The window.</param>
        internal static void ValidateMinimizedWindow(MDIWindow window)
        {
            if (window.IsMinimized)
            {
                window.IsPanelLayout = false;
                window.WasMinimizedDragged = true;
                window.ShowContextMenu = false;
            }
        }
        
        /// <summary>
        /// Gets the size value.
        /// </summary>
        /// <param name="winMinSize">Size of the win min.</param>
        /// <param name="size">The size DependencyObject.</param>
        /// <returns>double winMinSize</returns>
        private static double GetSizeValue(double winMinSize, double size)
        {
            if (!double.IsNaN(winMinSize))
            {
                return Math.Max(winMinSize, size);
            }

            return Math.Max(1, size);
        }
        
        /// <summary>
        /// Gets the cursor on border.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.QueryCursorEventArgs"/> instance containing the event data.</param>
        private static void GetCursorOnBorder(object sender, QueryCursorEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
             DependencyObject content=null;
             if (element.DataContext is DependencyObject)
             {
                 content = (DependencyObject)element.DataContext;
             }
             else
             {
                 content = element;
             }

            MDIWindow window = (MDIWindow)element.TemplatedParent;
            MDILayoutPanel panel = (MDILayoutPanel)window.Parent;

            if (panel != null)
            {
                bool allowResize = panel.Container.IsAllowMDIResize
                                   && DocumentContainer.GetAllowMDIResize(content);

                if (allowResize)
                {
                    Point point = e.GetPosition(element);
                    MDIBorder border = GetSide(element, point);
                    Cursor cursor = GetCursor(border, panel.FlowDirection);

                    if (cursor != null)
                    {
                        e.Handled = true;
                        e.Cursor = cursor;
                    }
                }
            }
        }
        #endregion
    }
}