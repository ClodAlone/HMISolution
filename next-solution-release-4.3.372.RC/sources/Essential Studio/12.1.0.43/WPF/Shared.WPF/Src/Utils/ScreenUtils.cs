// <copyright file="ScreenUtils.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Stores methods for work with monitor area.
    /// </summary>
    public class ScreenUtils
    {
        #region Public methods
        /// <summary>
        /// Correct given rect.
        /// </summary>
        /// <param name="rectWindow">Given <see cref="Rect"/>.</param>
        /// <returns>Corrected <see cref="Rect"/>.</returns>
        public static Rect FixByScreenBounds(Rect rectWindow)
        {
            if (rectWindow.IsEmpty)
            {
                return rectWindow;
            }

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)rectWindow.X, (int)rectWindow.Y, (int)rectWindow.Width, (int)rectWindow.Height);
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromRectangle(rect);

            if (screen!=null && System.Windows.Forms.Screen.AllScreens.Length==1)
            {
                System.Drawing.Rectangle rectVisible = screen.WorkingArea;

                if (rectVisible.Bottom <= rect.Y)
                {
                    rect.Y = rectVisible.Bottom - rect.Height;
                }

                if (rect.X >= rectVisible.Right)
                {
                    rect.X = rectVisible.Right - rect.Width;
                }
                else if (rect.X <= rectVisible.Left)
                {
                    rect.X = rectVisible.Left;
                }
           
            }
            else
            {
                Debug.WriteLine("No corresponding screen can be found");
            }

            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Gets element from point.
        /// </summary>
        /// <param name="parentItemsControl">Parent <see cref="ItemsControl"/> that contains a point</param>
        /// <param name="point">Given point.</param>
        /// <returns><see cref="FrameworkElement"/> from point.</returns>
        public static FrameworkElement GetElementFromPoint(ItemsControl parentItemsControl, Point point)
        {
            Rect rect;
            Point startPoint, endPoint;
            foreach (object child in parentItemsControl.Items)
            {
                UIElement element;

                if (child is UIElement)
                     element = child as UIElement;
                else
                     element = parentItemsControl.ItemContainerGenerator.ContainerFromItem(child) as UIElement;

                if (element != null)
                {
                    if (element.IsVisible)
                    {
                        startPoint = element.PointToScreen(new Point(0, 0));
                        endPoint = element.PointToScreen(new Point(element.RenderSize.Width, element.RenderSize.Height));
                        rect = new Rect(startPoint, endPoint);
                        if (rect.Contains(point))
                        {
                            return element as FrameworkElement;
                        }
                    }
                }
            }            

            return null;
        }
        #endregion
    }
}
