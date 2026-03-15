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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#if !WPF
using System.Diagnostics;
#else
using Syncfusion.Windows.Shared;
#endif

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class ToolBarManagerPanel: Panel
    {
        private ToolBarManager manager = null;

        /// <summary>
        /// 
        /// </summary>
        public ToolBarManager Manager
        {
            get
            {
                if (manager == null)
                {
#if !WPF
                    manager = VisualUtil.FindAncestor(this, typeof(ToolBarManager)) as ToolBarManager;
#else
                    manager = VisualUtils.FindAncestor(this, typeof(ToolBarManager)) as ToolBarManager;
#endif
                }

                return manager;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ToolBarManagerPanel()
        {

        }

        internal void Arrange(Size finalSize)
        {
            double y = 0;
            double x = 0;
            double height = 0;
            double width = 0;

            /*************** TOP TRAY RECT ****************/

            if (Manager.TopToolBarTray != null)
            {
                CalculateSize(Manager.TopToolBarTray, ref height);
            }

            Rect topRect = new Rect(0, 0, finalSize.Width, height);

            /*************** BOTTOM TRAY RECT ****************/

            height = 0;

            if (Manager.BottomToolBarTray != null)
            {
                CalculateSize(Manager.BottomToolBarTray, ref height);
            }

            y = finalSize.Height - height;

            Rect bottomRect = new Rect(0, Math.Max(0, y), finalSize.Width, height);

            /*************** LEFT TRAY RECT *****************/

            if (Manager.LeftToolBarTray != null)
            {
                CalculateSize(Manager.LeftToolBarTray, ref width);
            }

            height = finalSize.Height - (topRect.Height + bottomRect.Height);
            height = height < 0 ? 20 : height;
            Rect leftRect = new Rect(0, topRect.Height, width, height);

            /************** RIGHT TRAY RECT ****************/

            width = 0;

            if (Manager.RightToolBarTray != null)
            {
                CalculateSize(Manager.RightToolBarTray, ref width);
            }

            x = finalSize.Width - width;

            Rect rightRect = new Rect(Math.Max(0, x), topRect.Height, width, height);

            /************** ARRANGING THE TRAYS ***************/

            if (Manager.TopToolBarTray != null)
                Manager.TopToolBarTray.ArrangeCall(topRect);
            
            if(Manager.LeftToolBarTray != null)
                Manager.LeftToolBarTray.ArrangeCall(leftRect);

            if(Manager.RightToolBarTray != null)
                Manager.RightToolBarTray.ArrangeCall(rightRect);

            if(Manager.BottomToolBarTray != null)
                Manager.BottomToolBarTray.ArrangeCall(bottomRect);

            /*************** CALC REMAINING RECT AND ARRANGE CONTENT ****************/

            width = Math.Max(0.0, finalSize.Width - (leftRect.Width + rightRect.Width));
            height = Math.Max(0.0, finalSize.Height - (topRect.Height + bottomRect.Height));
            Rect remainingRect = new Rect(leftRect.Right, topRect.Bottom, width, height);

            Manager.content.Arrange(remainingRect);
            if(Manager.Content != null)
                Manager.Content.Arrange(new Rect(0, 0, width, height));
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Arrange(finalSize);

            return finalSize;
        }

        private void CalculateSize(ToolBarTrayAdv tray, ref double size)
        {
            foreach (ToolBarBand band in tray.Bands)
            {
                size += band.Size;
            }
        }
    }
}
