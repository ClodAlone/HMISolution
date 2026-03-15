#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicCellHelper
    {
        public static bool? GetIsInTop(DependencyObject obj)
        {
            return (bool?)obj.GetValue(IsInTopProperty);
        }

        public static void SetIsInTop(DependencyObject obj, bool? value)
        {
            obj.SetValue(IsInTopProperty, value);
        }

        public static readonly DependencyProperty IsInTopProperty =
            DependencyProperty.RegisterAttached("IsInTop", typeof(bool?), typeof(GraphicCellHelper), new PropertyMetadata(false));

        public static GraphicCellSpanInfo GetCellSpanInfo(DependencyObject obj)
        {
            return (GraphicCellSpanInfo)obj.GetValue(CellSpanInfoProperty);
        }

        public static void SetCellSpanInfo(DependencyObject obj, GraphicCellSpanInfo value)
        {
            obj.SetValue(CellSpanInfoProperty, value);
        }

        public static readonly DependencyProperty CellSpanInfoProperty =
            DependencyProperty.RegisterAttached("CellSpanInfo", typeof(GraphicCellSpanInfo), typeof(GraphicCellHelper), new PropertyMetadata(null));

        public static GraphicStyleInfo GetStyleInfo(DependencyObject obj)
        {
            return (GraphicStyleInfo)obj.GetValue(StyleInfoProperty);
        }

        public static void SetStyleInfo(DependencyObject obj, GraphicStyleInfo value)
        {
            obj.SetValue(StyleInfoProperty, value);
        }

        public static readonly DependencyProperty StyleInfoProperty =
            DependencyProperty.RegisterAttached("StyleInfo", typeof(GraphicStyleInfo), typeof(GraphicCellHelper), new PropertyMetadata(null));

        public static IGraphicCellRenderer GetGraphicCellRenderer(DependencyObject obj)
        {
            return (IGraphicCellRenderer)obj.GetValue(GraphicCellRendererProperty);
        }

        public static void SetGraphicCellRenderer(DependencyObject obj, IGraphicCellRenderer value)
        {
            obj.SetValue(GraphicCellRendererProperty, value);
        }

        public static readonly DependencyProperty GraphicCellRendererProperty =
            DependencyProperty.RegisterAttached("GraphicCellRenderer", typeof(IGraphicCellRenderer), typeof(GraphicCellHelper), new PropertyMetadata(null));

        public static bool? GetHandleMouseInput(DependencyObject obj)
        {
            return (bool?)obj.GetValue(HandleMouseInputProperty);
        }

        public static bool? GetHandleMouseInput(DependencyObject obj, UIElement falseIfParent)
        {
            while (obj.GetValue(HandleMouseInputProperty) == null)
            {
                DependencyObject parent = GridUtil.GetParent(obj);
                if (parent == falseIfParent)
                    return false;
                if (parent == null)
                    return null;
                obj = parent;
            }
            return (bool?)obj.GetValue(HandleMouseInputProperty);
        }

        public static void SetHandleMouseInput(DependencyObject obj, bool? value)
        {
            obj.SetValue(HandleMouseInputProperty, value);
        }

        public static readonly DependencyProperty HandleMouseInputProperty =
            DependencyProperty.RegisterAttached("HandleMouseInput", typeof(bool?), typeof(GraphicCellHelper), new PropertyMetadata(null));

        public static GraphicCellControl GetGraphicCellControl(DependencyObject obj)
        {
            return (GraphicCellControl)obj.GetValue(GraphicCellControlProperty);
        }

        public static void SetGraphicCellControl(DependencyObject obj, GraphicCellControl value)
        {
            obj.SetValue(GraphicCellControlProperty, value);
        }

        public static readonly DependencyProperty GraphicCellControlProperty =
            DependencyProperty.RegisterAttached("GraphicellControl", typeof(GraphicCellControl), typeof(GraphicCellHelper), new PropertyMetadata(null));
    }
}
