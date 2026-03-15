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
    public class GridDataCellRowHeaderRenderer : GridVirtualizingCellRenderer<Border>
    {
        public GridDataCellRowHeaderRenderer()
        {
            this.IsFocusable = true;
            this.AllowRecycle = false;
        }

        public override void OnInitializeContent(Border border, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(border, style);
            border.Background = new SolidColorBrush(Colors.Transparent);
            var model = (this.GridControl.Model as GridDataTableModel);
            if (model != null && model.TableProperties != null && model.TableProperties.StyleManager != null && model.TableProperties.StyleManager.RowAppearence != null && model.TableProperties.StyleManager.RowAppearence.RowHeaderIconPath != null)
                border.Child = model.TableProperties.StyleManager.RowAppearence.RowHeaderIconPath;
            else
                border.Child = GetPath(style.Foreground);
            border.BorderThickness = new Thickness(0);
            border.Padding = new Thickness(3); //Thickness is reduced due to get proper path.
        }


        private static Path GetPath(Brush foreground)
        {
            var path = XamlReader.Load(@"<Path xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""  Data=""M0,0 L8.8333235,7 L0,14 z"" />") as Path;
            path.Fill = foreground;
            path.Stroke = foreground;
            path.VerticalAlignment = VerticalAlignment.Center;
            path.HorizontalAlignment = HorizontalAlignment.Center;
            return path;
        }
    }

    public class GridDataBackgroundContentCellRenderer : GridVirtualizingCellRenderer<Border>
    {
        public GridDataBackgroundContentCellRenderer()
        {
            this.IsFocusable = true;
            this.AllowRecycle = false;
        }

        public GridDataTableModel TableModel
        {
            get
            {
                var gridDataTableModel = this.GridControl.Model as GridDataTableModel;
                return gridDataTableModel;
            }
        }       

        public override void OnInitializeContent(Border uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            var path = this.GetPath();
            if (path != null)
            {
                uiElement.Padding = new Thickness(5);
                uiElement.Child = path;
            }
                        
            uiElement.BorderThickness = new Thickness(0);
        }

        private Path GetPath()
        {
            var path = XamlReader.Load(@"<Path Width=""15"" Height=""15"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""  Data=""F1 M 403.083,282.99L 397.49,286.02L 403.083,289.068L 402.193,290.599L 396.943,287.427L 396.943,293.318L 395.146,293.318L 395.146,287.427L 389.895,290.599L 389.005,289.068L 394.599,286.02L 389.005,282.99L 389.895,281.443L 395.146,284.615L 395.146,278.724L 396.943,278.724L 396.943,284.615L 402.193,281.443L 403.083,282.99 Z "" Stretch=""Fill""/>") as Path;
            path.VerticalAlignment = VerticalAlignment.Center;
            path.HorizontalAlignment = HorizontalAlignment.Center;
            var visualStyle = this.TableModel.TableProperties.VisualStyle;
            switch (visualStyle)
            {
                case VisualStyle.Office2007Blue:
                case VisualStyle.Office2007Black:
                case VisualStyle.Blend:
                case VisualStyle.GlassyGreen:
                case VisualStyle.VS2010:
                     path.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case VisualStyle.Office2003:
                    path.Fill = new SolidColorBrush(Colors.White);
                    break;
                case VisualStyle.Office2007Silver:
                default:
                    path.Fill = new SolidColorBrush(Colors.Black);
                    break;
            }         
            
            return path;
        }
    }
}
