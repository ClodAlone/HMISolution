// <copyright file="DrawingUtils.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Interop;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Presents static class for drawing process purpose.
    /// </summary>
    public static class DrawingUtils
    {
        #region Private members

        /// <summary>
        /// Presents static empty point.
        /// </summary>
        private readonly static Point EMPTY_POINT = new Point(0, 0);

        /// <summary>
        /// Represents WFC flag.
        /// </summary>
        private static bool wfcHostFlag = false;

        #endregion

        #region Public methods

        /// <summary>
        /// Prepares the fake.
        /// </summary>
        /// <param name="mainElement">The main element.</param>
        /// <returns>Drawing brush from drawing group.</returns>
        public static DrawingBrush PrepareFake(UIElement mainElement)
        {
            Point mainPoint = PermissionHelper.GetSafePointToScreen(mainElement, EMPTY_POINT);
            DrawingGroup mainGroup = GetDrawingGroup(mainElement, mainPoint);
            return new DrawingBrush(mainGroup);
        }

        private static FrameworkElement m_winhost;
        
        /// <summary>
        /// Prepares the fake.
        /// </summary>
        /// <param name="mainElement">The main element.</param>
        /// <param name="fake">The fake border.</param>
        public static void PrepareFake(FrameworkElement mainElement, Border fake)
        {
            Point mainPoint = PermissionHelper.GetSafePointToScreen(mainElement, EMPTY_POINT);
            m_winhost = mainElement;
            DrawingGroup mainGroup = GetDrawingGroup(mainElement, mainPoint);
            if(m_winhost!=null)
            fake.FlowDirection = m_winhost.FlowDirection;
            fake.Height = mainElement.ActualHeight;
            fake.Width = mainElement.ActualWidth;
            fake.Margin = mainElement.Margin;
            fake.Background = new DrawingBrush(mainGroup);
        }

        /// <summary>
        /// Gets the screen shot.
        /// </summary>
        /// <param name="winHandle">The handle of the window, from which screenshot have to be made.</param>
        /// <param name="bounds">The bounds of the screenshot.</param>
        /// <returns>Bitmap from select object.</returns>
        public static System.Drawing.Bitmap GetScreenShot(IntPtr winHandle, Size bounds)
        {
            if (winHandle == IntPtr.Zero)
            {
                throw new ArgumentException("Window handle is invalid!");
            }

            System.Drawing.Bitmap bm = null;

            IntPtr hDC = NativeMethods.GetDC(winHandle);
            IntPtr m_hmemDC = NativeMethods.CreateCompatibleDC(hDC);

            int width = (int)bounds.Width;
            int height = (int)bounds.Height;

            IntPtr m_hbitmap = NativeMethods.CreateCompatibleBitmap(hDC, width, height);

            if (m_hbitmap != IntPtr.Zero)
            {
                IntPtr m_hold = (IntPtr)NativeMethods.SelectObject(m_hmemDC, m_hbitmap);

                NativeMethods.BitBlt(m_hmemDC, 0, 0, width, height, hDC, 0, 0, (int)NativeMethods.TernaryRasterOperations.SRCCOPY);

                NativeMethods.SelectObject(m_hmemDC, m_hold);
                NativeMethods.DeleteDC(m_hmemDC);
                NativeMethods.ReleaseDC(winHandle, hDC);
                bm = System.Drawing.Image.FromHbitmap(m_hbitmap);
                NativeMethods.DeleteObject(m_hbitmap);
            }

            return bm;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Gets the drawing group.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="mainPoint">The main point.</param>
        /// <returns>DrawingGroup from element.</returns>
        private static DrawingGroup GetDrawingGroup(UIElement element, Point mainPoint)
        {
            List<Drawing> lsitDrawing = new List<Drawing>();
            Drawing drawing = GetDrawing(element, mainPoint);
            if (!element.GetType().Name.Equals("DockedElementTabbedHost"))
            {
                lsitDrawing.Add(drawing);
            }

            wfcHostFlag = false;
            GetDrawingTree(element, lsitDrawing, mainPoint, true);
            DrawingGroup mainGroup = new DrawingGroup();

            for (int i = 0, cnt = lsitDrawing.Count; i < cnt; ++i)
            {
                mainGroup.Children.Add(lsitDrawing[i]);
            }

            return mainGroup;
        }

        /// <summary>
        /// Gets the drawing.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="mainPoint">The main point.</param>
        /// <returns>Drawing from screen.</returns>
        private static Drawing GetDrawing(UIElement element, Point mainPoint)
        {
            Point elementPoint = PermissionHelper.GetSafePointToScreen(element, EMPTY_POINT);
            Point setPoint = new Point(elementPoint.X - mainPoint.X, elementPoint.Y - mainPoint.Y);
            return element is HwndHost
                ? GetDrawingUseVisualTreeHelper(element, setPoint)
                : GetDrawingUseDrawingVisual(element, setPoint);
        }

        /// <summary>
        /// Gets the drawing use drawing visual.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="setPoint">The set point.</param>
        /// <returns>Drawing from drawing visual.</returns>
        private static Drawing GetDrawingUseDrawingVisual(UIElement element, Point setPoint)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(element);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(setPoint, element.RenderSize));
            }

            return drawingVisual.Drawing;
        }

        /// <summary>
        /// Gets the drawing use visual tree helper.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="setPoint">The set point.</param>
        /// <returns>Drawing from drawing group.</returns>
        private static Drawing GetDrawingUseVisualTreeHelper(UIElement element, Point setPoint)
        {
            Drawing result = null;
            DrawingGroup drawGroup = VisualTreeHelper.GetDrawing(element);

            if (null != drawGroup)
            {
                ImageDrawing imageDrawing = (ImageDrawing)drawGroup.Children[0];
                Rect rect = new Rect(setPoint, element.RenderSize);
                result = new ImageDrawing(imageDrawing.ImageSource, rect);
            }

            return result;
        }

        /// <summary>
        /// Gets the drawing tree.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="drawingList">The drawing list.</param>
        /// <param name="mainPoint">The main point.</param>
        /// <param name="isFirstBranch">if set to <c>true</c> [is first branch].</param>
        /// <returns>Boolean value</returns>
        private static bool GetDrawingTree(DependencyObject element, List<Drawing> drawingList, Point mainPoint, bool isFirstBranch)
        {
            bool isWFH = false;
            int icount = VisualTreeHelper.GetChildrenCount(element);
            List<FrameworkElement> children = new List<FrameworkElement>();

            for (int i = 0; i < icount; ++i)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (null != child)
                {
                    isWFH |= child is HwndHost;
                    children.Add(child);
                }
            }

            bool childrenHasWFH = false;
            int childrenCount = children.Count;
            List<Drawing> childenDrawing = new List<Drawing>();

            for (int i = 0; i < childrenCount; ++i)
            {
                FrameworkElement child = children[i];
                Drawing drawing = GetDrawing(child, mainPoint);
                if (child is HwndHost)
                {
                    FrameworkElement childelement = element as FrameworkElement;
                    if (m_winhost != null && childelement!=null && m_winhost.FlowDirection != childelement.FlowDirection)
                    {
                        DrawingGroup ReverseDrawing = new DrawingGroup();
                        ReverseDrawing.Children.Add(drawing);
                        ScaleTransform flipTransform = new ScaleTransform();
                        flipTransform.ScaleX = -1;
                        ReverseDrawing.Transform = flipTransform;
                        Point elementPoint = PermissionHelper.GetSafePointToScreen(child, EMPTY_POINT);
                        Point setPoint = new Point(elementPoint.X - mainPoint.X, elementPoint.Y - mainPoint.Y);
                        Rect rect = new Rect(setPoint, child.RenderSize);
                        DrawingImage drawingImageSource = new DrawingImage(ReverseDrawing);
                        Drawing flipdrawing = new ImageDrawing(drawingImageSource, rect);
                        drawingList.Add(flipdrawing);
                    }
                    else
                    {
                        drawingList.Add(drawing);
                    }
                    wfcHostFlag = true;
                }
                
                if (!isWFH && !wfcHostFlag)
                {
                    childrenHasWFH |= GetDrawingTree(child, childenDrawing, mainPoint, false);
                }
            }

            if (isFirstBranch || childrenHasWFH)
            {
                drawingList.AddRange(childenDrawing);
            }

            return isWFH || childrenHasWFH;
        }
        #endregion
    }
}