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
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicCheckBoxCellModel : GraphicCellModel<GraphicCheckBoxCellRenderer>
    {
        public GraphicCheckBoxCellModel()
        {

        }
    }

    public class GraphicCheckBoxCellRenderer : GraphicCellRendererBase<CheckBox>
    {
        public GraphicCheckBoxCellRenderer()
        {
            IsEditable = true;
        }

        protected override CheckBox CreateUIElement(GraphicStyleInfo cellInfo)
        {
            if (cellInfo.CellValue != null)
            {
                CheckBox checkbox = new CheckBox();
                bool result = false;
                string cellvalue = cellInfo.CellValue.ToString();
                if (bool.TryParse(cellvalue, out result))
                {
                    checkbox.IsChecked = result;
                }
                checkbox.Content = cellInfo.Text;
                return checkbox;
            }
            return base.CreateUIElement(cellInfo);
        }

        protected override void OnInitializeContent(CheckBox element, GraphicStyleInfo style)
        {
            if (style.HasHorizontalAlignment)
                element.HorizontalAlignment = style.HorizontalAlignment;
            if (style.HasVerticalAlignment)
                element.VerticalAlignment = style.VerticalAlignment;
            base.OnInitializeContent(element, style);
        }

        protected override object GetControlValueFromEditor(CheckBox uiElement)
        {
            if (uiElement != null)
            {
                return uiElement.IsChecked;
            }
            return base.GetControlValueFromEditor(uiElement);
        }

        protected override void WireEvents(CheckBox element)
        {
#if !SILVERLIGHT
            element.PreviewMouseDown += new MouseButtonEventHandler(element_PreviewMouseDown);
#else
            element.MouseLeftButtonDown += new MouseButtonEventHandler(element_MouseLeftButtonDown);
#endif
        }

        void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        
        void element_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var graphicCellControl = GraphicCellHelper.GetGraphicCellControl((sender as UIElement));
            var checkbox = sender as CheckBox;
            if (Keyboard.Modifiers == ModifierKeys.Control || graphicCellControl.IsSelected)
            {
                var spanInfo = GraphicCellHelper.GetCellSpanInfo(sender as UIElement);
                this.CellModel.GraphicModel.SelectionController.OnMouseDown(e, spanInfo);
                e.Handled = true;
            }
            else
            {
                if (this.CellModel.GraphicModel.SelectedGraphicCells.Count > 0)
                {
                    var span = this.CellModel.GraphicModel.SelectedGraphicCells[0];
                    var control = this.CellModel.GraphicModel[span.CellSpanIndex].GraphicCellControl;
                    control.Focus();
                }
                else
                    this.GridControl.Focus();
            }
        }

        protected override void UnWireEvents(CheckBox element)
        {
#if !SILVERLIGHT
            element.PreviewMouseDown -= new MouseButtonEventHandler(element_PreviewMouseDown);
#else
            element.MouseLeftButtonDown -= new MouseButtonEventHandler(element_MouseLeftButtonDown);
#endif
        }

    }

}
