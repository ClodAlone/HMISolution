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
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicImageCellModel:GraphicCellModel<GraphicImageCellRenderer>
    {
        public GraphicImageCellModel()
        {

        }
    }

    public class GraphicImageCellRenderer : GraphicCellRendererBase<Image>
    {
        public GraphicImageCellRenderer()
        {

        }

        protected override Image CreateUIElement(GraphicStyleInfo cellInfo)
        {
            if (cellInfo.CellValue != null && cellInfo.CellValue is BitmapImage)
            {
                Image image = new Image();
                BitmapImage bitImg = cellInfo.CellValue as BitmapImage;
                image.Source = bitImg;
                image.Stretch = System.Windows.Media.Stretch.Fill;
                return image;
            }
            return base.CreateUIElement(cellInfo);
        }

        protected override void OnInitializeContent(Image element, GraphicStyleInfo style)
        {
            base.OnInitializeContent(element, style);
        }
    }
}
