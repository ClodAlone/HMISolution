#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Controls.Grid;

    /// <summary>
    /// Implements the Row header Cell Model for Grid Data Control.
    /// </summary>
    public class GridDataCellRowHeaderModel : GridCellModel<GridDataCellRowHeaderRenderer>
    {
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            return new Size(GridDataTableModel.ExpandCollapseCellWidth, GridDataTableModel.HeaderRowHeight);
        }
    }

    /// <summary>
    /// Provides the drawing for Row header cells.
    /// </summary>
    public class GridDataCellRowHeaderRenderer : GridCellRendererBase
    {
        public GridDataCellRowHeaderRenderer()
        {
        }

        protected override void OnRender(DrawingContext dc, Syncfusion.Windows.Controls.Cells.RenderCellArgs rca, GridRenderStyleInfo style)
        {
            bool applyOldSkin = false;
            if (this.GridControl.Model is GridDataTableModel)
                applyOldSkin = (this.GridControl.Model as GridDataTableModel).TableProperties.IsLegacyStyleEnabled;
            else if (this.GridControl.Model is GridDataGroupDropAreaModel)
                applyOldSkin = (this.GridControl.Model as GridDataGroupDropAreaModel).TableProperties.IsLegacyStyleEnabled;

            Rect textRectangle = applyOldSkin ? rca.CellRect : GetCenteredRect(rca.CellRect, 17);
            var model = (this.GridControl.Model as GridDataTableModel);
            Border border;
            if (model != null && model.TableProperties != null && model.TableProperties.StyleManager != null && model.TableProperties.StyleManager.RowAppearence != null && model.TableProperties.StyleManager.RowAppearence.RowHeaderIconPath != null)
            {
                if (model.TableProperties.StyleManager.RowAppearence.RowHeaderIconPath.Parent != null)
                    ((model.TableProperties.StyleManager.RowAppearence.RowHeaderIconPath.Parent) as Border).Child = null;
               border = new Border() { Child = model.TableProperties.StyleManager.RowAppearence.RowHeaderIconPath };
            }
            else
                border = this.GetContent(textRectangle, style.Background, style.Foreground);
            var vBrush = new VisualBrush(border);
            dc.DrawRectangle(vBrush, null, textRectangle);
            
        }

        private Rect GetCenteredRect(Rect rect, double minWidth)
        {
            double minSize = Math.Min(rect.Width, rect.Height);
            double calWidth = minWidth;
            double calHeight = minWidth;
            if (rect.Width < minWidth)
            {
                calWidth = minSize;
            }

            if (rect.Height < minWidth)
            {
                calHeight = minSize;
            }

            double calX = rect.X + ((rect.Width - calWidth) / 2);
            double calY = rect.Y + ((rect.Height - calHeight) / 2);

            return new Rect(calX, calY, calWidth, calHeight);
        }

        private Border GetContent(Rect rect, Brush background, Brush foreground)
        {
            var border = new Border();
            border.Width = rect.Width;
            border.Height = rect.Height;
            border.Measure(new Size(rect.Width, rect.Height));
            VisualContainer.SetWantsMouseInput(border, false);
            var innerBorder = new Border();
            innerBorder.Background = this.GetBackground(background, foreground);
            //innerBorder.Margin = new Thickness(4, 5, 3, 5);
            border.Child = innerBorder;
            border.BorderThickness = new Thickness(0);
            border.Padding = new Thickness(5);
            border.Measure(new Size(rect.Width, rect.Height));
            return border;
        }

        private Brush GetBackground(Brush background, Brush foreground)
        {
            // <DrawingBrush x:Key="Layer_1">
            //    <DrawingBrush.Drawing>
            //        <DrawingGroup>
            //            <DrawingGroup.Children>
            //                <GeometryDrawing Brush="#FF000019" Geometry="F1 M 398.744,310.511L 398.744,294.823L 406.589,302.667L 398.744,310.511 Z "/>
            //                <GeometryDrawing Brush="#FF6F7DA4" Geometry="F1 M 405.931,302.667L 399.209,309.389L 399.209,295.946L 405.931,302.667 Z "/>
            //            </DrawingGroup.Children>
            //        </DrawingGroup>
            //    </DrawingBrush.Drawing>
            // </DrawingBrush>
            var brush = new DrawingBrush()
            {
                Drawing = new DrawingGroup()
                {
                    Children = new DrawingCollection()
                    {
                        new GeometryDrawing()
                        {
                            Brush = background,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 398.744,310.511L 398.744,294.823L 406.589,302.667L 398.744,310.511 Z ")
                        },
                        new GeometryDrawing()
                        {
                            Brush = foreground,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 405.931,302.667L 399.209,309.389L 399.209,295.946L 405.931,302.667 Z ")
                        }
                    }
                }
            };
            brush.AlignmentX = AlignmentX.Center;
            brush.AlignmentY = AlignmentY.Center;
            brush.Stretch = Stretch.Uniform;
            return brush;
        }
    }

    /// <summary>
    /// Provides the drawing visual for add new row header cells.
    /// </summary>
    public class GridDataAddNewRowContentCellRenderer : GridVirtualizingCellRenderer<Border>
    {
        public GridDataAddNewRowContentCellRenderer()
        {
        }

        public GridDataTableModel TableModel
        {
            get
            {
                var gridDataTableModel = this.GridControl.Model as GridDataTableModel;
                return gridDataTableModel;
            }
        }

        private Brush GetBrush()
        {
            Brush brush = null;
            var visualStyle = this.TableModel.TableProperties.VisualStyle;
            switch (visualStyle)
            {
                case VisualStyle.Office2007Blue:
                    brush = GridDataResourceWrapper.WhiteAsterisk;
                    break;
                case VisualStyle.Office2007Silver:
                    brush = GridDataResourceWrapper.BlackAsterisk;
                    break;
                case VisualStyle.Office2007Black:
                    brush = GridDataResourceWrapper.WhiteAsterisk;
                    break;
                case VisualStyle.Blend:
                    brush = GridDataResourceWrapper.WhiteAsterisk;
                    break;
                case VisualStyle.Office2003:
                    brush = GridDataResourceWrapper.WhiteAsterisk;
                    break;
                // case VisualStyle.Default:
                default:
                    brush = GridDataResourceWrapper.BlackAsterisk;
                    break;
            }

            return brush;
        }

        protected override void OnRender(DrawingContext dc, Syncfusion.Windows.Controls.Cells.RenderCellArgs rca, GridRenderStyleInfo style)
        {
            Rect textRectangle = rca.CellRect;
            textRectangle.Inflate(-5, -5);
            var border = this.GetContent(textRectangle, style.Background, style.Foreground);
            var vBrush = new VisualBrush(border);
            dc.DrawRectangle(vBrush, null, textRectangle);
        }

        private Border GetContent(Rect rect, Brush background, Brush foreground)
        {
            //if ((!(double.IsInfinity)(rect.Width)) && (!(double.IsInfinity)(rect.Height)))
            //{
                var border = new Border();
                border.Background = this.GetBrush();
                border.Width = rect.Width;
                border.Height = rect.Height;
                return border;
            //}
            //else
            //    return null;
        }
    }    
}
