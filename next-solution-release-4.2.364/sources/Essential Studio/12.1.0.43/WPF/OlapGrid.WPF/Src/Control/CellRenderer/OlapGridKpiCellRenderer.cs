#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Grid.Olap
#else
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Syncfusion.Windows.Controls.Grid;

    public class OlapGridKpiCellModel : GridCellModel<OlapGridKpiCellRenderer>
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
            return new System.Windows.Size(10, 10);
        }
    }

    public class OlapGridKpiCellRenderer : GridVirtualizingCellRenderer<OlapGridKpiCell>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridKpiCellRenderer"/> class.
        /// </summary>
        public OlapGridKpiCellRenderer()
        {
            this.AllowRecycle = true;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Creates the renderer element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void CreateRendererElement(OlapGridKpiCell uiElement, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(uiElement, style);
            OlapGridCellStyleInfoIdentity cellIdentity = style.ModelStyle.Tag as OlapGridCellStyleInfoIdentity;

#if !SILVERLIGHT
            if (cellIdentity != null)
            {
                uiElement.CellDescriptor = cellIdentity.CellDescriptor;
                uiElement.Style = cellIdentity.Style;
            }
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                uiElement.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = uiElement.ActualWidth;
                double offsetY = 0;
                uiElement.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY);
            }
#else
            if (cellIdentity.CellDescriptor != null)
            {

                uiElement.CellDescriptor = cellIdentity.CellDescriptor;
                /// Setting the KPI image
                if (cellIdentity.CellDescriptor.KpiType == Syncfusion.OlapSilverlight.Data.KpiTypeEnum.Kpi_Trend)
                {
                    if (cellIdentity.CellDescriptor.CellValue == "-1")
                    {
                        uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/DownArrow.png";
                    }
                    else if (cellIdentity.CellDescriptor.CellValue == "0")
                    {
                        uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/RightArrow.png";
                    }
                    else if (cellIdentity.CellDescriptor.CellValue == "1")
                    {
                        uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/UpArrow.png";
                    }
                }
                else if (cellIdentity.CellDescriptor.KpiType == Syncfusion.OlapSilverlight.Data.KpiTypeEnum.Kpi_Status)
                {
                    if (cellIdentity.CellDescriptor.KpiGraphicsStyle == "Road Signs")
                    {
                        if (cellIdentity.CellDescriptor.CellValue == "-1")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/Red.png";
                        }
                        else if (cellIdentity.CellDescriptor.CellValue == "0")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/ThreeColor.png";
                        }
                        else if (cellIdentity.CellDescriptor.CellValue == "1")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/Green.png";
                        }
                    }
                    else
                    {
                        if (cellIdentity.CellDescriptor.CellValue == "-1")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/Diamond.png";
                        }
                        else if (cellIdentity.CellDescriptor.CellValue == "0")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/Triangle.png";
                        }
                        else if (cellIdentity.CellDescriptor.CellValue == "1")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/Circle.png";
                        }
                    }
                }
            }
#endif
        } 
#endif

        /// <summary>
        /// Called when [initialize content].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void OnInitializeContent(OlapGridKpiCell uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            OlapGridCellStyleInfoIdentity cellIdentity = style.ModelStyle.Tag as OlapGridCellStyleInfoIdentity;

#if !SILVERLIGHT
            if (cellIdentity != null)
            {
                uiElement.CellDescriptor = cellIdentity.CellDescriptor;
                uiElement.Style = cellIdentity.Style;
            }
#else
            if (cellIdentity.CellDescriptor != null)
            {

                uiElement.CellDescriptor = cellIdentity.CellDescriptor;
                /// Setting the KPI image
                if (cellIdentity.CellDescriptor.KpiType == Syncfusion.OlapSilverlight.Data.KpiTypeEnum.Kpi_Trend)
                {
                    if (cellIdentity.CellDescriptor.CellValue == "-1")
                    {
                        uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/DownArrow.png";
                    }
                    else if (cellIdentity.CellDescriptor.CellValue == "0")
                    {
                        uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/RightArrow.png";
                    }
                    else if (cellIdentity.CellDescriptor.CellValue == "1")
                    {
                        uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/UpArrow.png";
                    }
                }
                else if (cellIdentity.CellDescriptor.KpiType == Syncfusion.OlapSilverlight.Data.KpiTypeEnum.Kpi_Status)
                {
                    if (cellIdentity.CellDescriptor.KpiGraphicsStyle == "Road Signs")
                    {
                        if (cellIdentity.CellDescriptor.CellValue == "-1")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/Red.png";
                        }
                        else if (cellIdentity.CellDescriptor.CellValue == "0")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/ThreeColor.png";
                        }
                        else if (cellIdentity.CellDescriptor.CellValue == "1")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/Green.png";
                        }
                    }
                    else
                    {
                        if (cellIdentity.CellDescriptor.CellValue == "-1")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/Diamond.png";
                        }
                        else if (cellIdentity.CellDescriptor.CellValue == "0")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/Triangle.png";
                        }
                        else if (cellIdentity.CellDescriptor.CellValue == "1")
                        {
                            uiElement.ImageSource = @"/Syncfusion.OlapGrid.Silverlight;component/Images/KPI/Circle.png";
                        }
                    }
                }
            }
#endif
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
