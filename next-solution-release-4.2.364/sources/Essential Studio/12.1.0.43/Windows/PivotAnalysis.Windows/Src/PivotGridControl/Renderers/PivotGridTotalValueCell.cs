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
    public class PivotGridTotalValueCellModel : GridTextBoxCellModel
    {
        public PivotGridTotalValueCellModel(GridModel grid)
            : base(grid)
        {
        }
        protected PivotGridTotalValueCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new PivotGridTotalValueCellRenderer(control, this);
        }
    }

    public class PivotGridTotalValueCellRenderer : GridTextBoxCellRenderer
    {
        public PivotGridTotalValueCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
        }
        protected override void OnOutlineCurrentCell(Graphics g, Rectangle r)
        {

        }
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {

        }
        protected override void OnDrawCellBackground(GridDrawCellBackgroundEventArgs e)
        {
            base.OnDrawCellBackground(e);
        }
        protected override void DrawBackground(Graphics g, Rectangle rect, GridStyleInfo style, bool fillBackground)
        {
            //style.BackColor = Color.FromArgb(251, 226, 146);
            //fillBackground = true;
            base.DrawBackground(g, rect, style, fillBackground);
        }
    }

}