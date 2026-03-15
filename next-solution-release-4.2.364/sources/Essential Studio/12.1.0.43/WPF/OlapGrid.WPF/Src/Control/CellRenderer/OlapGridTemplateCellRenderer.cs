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
using Syncfusion.Windows.Controls.Grid;
using System.Windows;

#if !SILVERLIGHT
using System.Windows.Media;
namespace Syncfusion.Windows.Grid.Olap
#else
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    public class OlapGridTemplateCellModel : GridCellModel<OlapGridTemplateCellRenderer>
    {
        /// <summary>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">Vertical or horizontal</param>
        /// <returns>The optimal size of the cell.</returns>
        /// <overload>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </overload>
        public override System.Windows.Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            OlapGridCellStyleInfoIdentity styleInfoIdentity = style.Tag as OlapGridCellStyleInfoIdentity;
            if (styleInfoIdentity != null)
            {
                if (styleInfoIdentity.Style != null)
                {
                    OlapGridTemplateCell cellControl = new OlapGridTemplateCell();
                    cellControl.CellDescriptor = styleInfoIdentity.CellDescriptor;
                    cellControl.DataContext = styleInfoIdentity.CellDescriptor;
                    cellControl.Style = styleInfoIdentity.Style;
                    cellControl.Measure(new Size(double.MaxValue, double.MaxValue));
                    return cellControl.DesiredSize;
                }
            }
            return new System.Windows.Size(10, 10);
        }

        /// <summary>
        /// Gets the formatted text.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="value">The value.</param>
        /// <param name="textInfo">The text info.</param>
        /// <returns></returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            var cellIdentity = style.Tag as OlapGridCellStyleInfoIdentity;
            return cellIdentity.CellDescriptor.CellValue;
        }
    }
    public class OlapGridTemplateCellRenderer : GridVirtualizingCellRenderer<OlapGridTemplateCell>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridTemplateCellRenderer"/> class.
        /// </summary>
        public OlapGridTemplateCellRenderer()
        {
            this.IsEditable = false;
            this.AllowRecycle = true;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Creates the renderer element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void CreateRendererElement(OlapGridTemplateCell uiElement, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(uiElement, style);
            OlapGridCellStyleInfoIdentity cellIdentity = style.ModelStyle.Tag as OlapGridCellStyleInfoIdentity;
            if (cellIdentity != null)
            {
                uiElement.CellDescriptor = cellIdentity.CellDescriptor;
                uiElement.Style = cellIdentity.Style;
                uiElement.DataContext = cellIdentity.CellDescriptor;
                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.HorizontalAlignment = HorizontalAlignment.Left;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;
#if !SILVERLIGHT
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                uiElement.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = uiElement.ActualWidth;
                double offsetY = 0;
                uiElement.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY);
            } 
#endif
                UnwireAndWireExpandClicked(uiElement);
            }
        }
#endif
        /// <summary>
        /// Called when [initialize content].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void OnInitializeContent(OlapGridTemplateCell uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            OlapGridCellStyleInfoIdentity cellIdentity = style.ModelStyle.Tag as OlapGridCellStyleInfoIdentity;
            if (cellIdentity != null)
            {
                uiElement.CellDescriptor = cellIdentity.CellDescriptor;
                uiElement.Style = cellIdentity.Style;
                uiElement.DataContext = cellIdentity.CellDescriptor;
                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.HorizontalAlignment = HorizontalAlignment.Left;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;
                UnwireAndWireExpandClicked(uiElement);
            }           
        }

        /// <summary>
        /// Unwires the and wire expand clicked.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        private void UnwireAndWireExpandClicked(OlapGridTemplateCell uiElement)
        {
            uiElement.ExpanderClicked -= new OlapGridDrillDownEventHander(uiElement_ExpanderClicked);
            uiElement.ExpanderClicked += new OlapGridDrillDownEventHander(uiElement_ExpanderClicked);
        }

        /// <summary>
        /// Handles the ExpanderClicked event of the uiElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        void uiElement_ExpanderClicked(object sender, OlapGridDrillDownEventArgs e)
        {
            OlapGridBase gridBase = this.GridControl as OlapGridBase;
            if (gridBase != null)
            {
#if SILVERLIGHT
                gridBase.OlapGrid.IsProcessing = true;
#endif
                if (gridBase.Model != null)
                {
                    gridBase.Model.GridExpanderClick(e.CellDescriptor, e);
                }
            }
        }

        /// <summary>
        /// Called when [element measured].
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="size">The size.</param>
        protected override void OnElementMeasured(System.Windows.UIElement el, System.Windows.Size size)
        {
            el.Dispatcher.BeginInvoke(new Action(() =>
            {
                el.Measure(size);
            }), null);
        }

        /// <summary>
        /// Called when [element arranged].
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="rect">The rect.</param>
        protected override void OnElementArranged(System.Windows.UIElement el, System.Windows.Rect rect)
        {
            el.Dispatcher.BeginInvoke(new Action(() =>
            {
                el.Arrange(rect);
            }), null);
            base.OnElementArranged(el, rect);
        }
    }
}
