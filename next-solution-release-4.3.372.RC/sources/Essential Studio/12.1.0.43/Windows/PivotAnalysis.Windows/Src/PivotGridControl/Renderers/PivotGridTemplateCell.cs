#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Drawing;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Windows.Forms.Tools;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    public class PivotGridTemplateCellModel :GridHeaderCellModel
    {
        public PivotGridTemplateCellModel(GridModel grid)
            : base(grid)
        {
        }
        protected PivotGridTemplateCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new PivotGridTemplateCellRenderer(control, this);
        }
    }

    public class PivotGridTemplateCellRenderer : GridHeaderCellRenderer
    {
        PivotGridControl baseGrid;
        public PivotGridTemplateCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            if (grid is GroupBar)
                baseGrid = (grid as GroupBar).GridControl;
            else if (grid is RowGroupBar)
                baseGrid = (grid as RowGroupBar).GridControl;
        }

        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
             Color color = SystemColors.Window;
             if (baseGrid != null)
             {
                 switch (this.baseGrid.GridVisualStyles)
                 {
                     case GridVisualStyles.Office2007Blue:
                     case GridVisualStyles.Office2010Blue:
                         color = Color.LightBlue;
                         break;

                     case GridVisualStyles.Office2010Black:
                         color = Color.LightGray;
                         break;

                     case GridVisualStyles.Office2007Silver:
                     case GridVisualStyles.Office2010Silver:
                     case GridVisualStyles.Office2007Black:
                         color = Color.Silver;
                         break;

                     case GridVisualStyles.Metro:
                         color = Color.Gray;
                         break;
                 }
             }
            style.Borders.All = new GridBorder(GridBorderStyle.Solid, color);
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }
        

    }

}