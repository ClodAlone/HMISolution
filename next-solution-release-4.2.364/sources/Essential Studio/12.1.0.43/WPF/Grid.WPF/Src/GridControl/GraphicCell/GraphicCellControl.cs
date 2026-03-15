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
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Grid;
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Media;
using System.Windows.Input;
using System.Diagnostics;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Syncfusion.Windows.Controls.Grid
{
    [DesignTimeVisible(false)]
    [ContentProperty("Content")]
    public class GraphicCellControl : Control
    {
        private GridControlBase grid;
        private GraphicStyleInfo cellInfo;
        private GraphicCellSpanInfo cellSpanInfo;
        private GraphicModel graphicModel;

        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(GraphicCellControl), new PropertyMetadata(null));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(GraphicCellControl), new PropertyMetadata(false));


        public GraphicCellControl(GridControlBase grid, GraphicStyleInfo cellInfo, GraphicCellSpanInfo cellSpanInfo)
        {
            DefaultStyleKey = typeof(GraphicCellControl);
#if !SILVERLIGHT
            FocusVisualStyle = null;
#endif
            this.grid = grid;
            this.graphicModel = grid.Model.GraphicModel;
            this.cellInfo = cellInfo;
            this.cellSpanInfo = cellSpanInfo;
        }
       

        protected override void OnKeyDown(KeyEventArgs e)
        {
#if !SILVERLIGHT
            if (this.IsFocused)
            {
                this.graphicModel.SelectionController.OnKeyDown(e, this.cellSpanInfo);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
                this.Focus();
#else
            if (FocusManager.GetFocusedElement() as UIElement == this as UIElement)
                this.graphicModel.SelectionController.OnKeyDown(e, this.cellSpanInfo);
            else if (e.Key == Key.Escape)
                (this as Control).Focus();
#endif

        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            e.Handled = true;
        }

#if !SILVERLIGHT
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            graphicModel.SelectionController.OnMouseDown(e, this.cellSpanInfo);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            e.Handled = true;
        }
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            graphicModel.SelectionController.OnMouseDown(e, this.cellSpanInfo);
            e.Handled = true;
        }
#endif
       
    }

#if SILVERLIGHT
    [DesignTimeVisible(false)]
    public class DecoratorControl : Control 
    { 
    }
#endif
}
