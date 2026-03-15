#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
   public class WinLossRender
    {
        public List<object> List = new List<object>();

        private int areaMarginX = 3;
        private int areaMarginY = 3;
        SparkLineSource m_sparklineSource = new SparkLineSource();

        /// <summary>
        /// Renderer the WinLoss type series
        /// </summary>
        /// <param name="og">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <param name="sparkline"></param>
        public void DrawSparkWinLoss(Graphics og,ISparkLine sparkline)
        {
            List = (List<object>)m_sparklineSource.GetSourceList(sparkline.Source, sparkline);

            if (List.Count > 0)
            {
                double areaWidth = sparkline.ControlWidth - areaMarginX * 2;
                double areaHeight = sparkline.ControlHeight - areaMarginY * 2;

                double areaHeightCenter = areaHeight / 2;
                double sInterval = areaWidth / (List.Count);
                double firstPointX = 0;

                for (int i = 0; i < List.Count; i++)
                {
                    double Value = Convert.ToDouble(List[i]);
                    firstPointX = sInterval * i + (sInterval / 2);

                    if (Value > 0)
                    {
                        Rectangle rect = new Rectangle((int)(areaMarginX + firstPointX - sInterval / 2 + sparkline.ColumnStyle.ColumnSpace), (int)(areaMarginY), (int)(sInterval - sparkline.ColumnStyle.ColumnSpace * 2), (int)(areaHeightCenter));
                        BrushInfo interior = GetInteriorWinLoss(Value,sparkline);
                        BrushPaint.FillRectangle(og, rect, interior);
                    }
                    else
                    {
                        Rectangle rect = new Rectangle((int)(areaMarginX + firstPointX - sInterval / 2 + sparkline.ColumnStyle.ColumnSpace), (int)(areaHeightCenter + areaMarginY), (int)(sInterval - sparkline.ColumnStyle.ColumnSpace * 2), (int)(areaHeightCenter - areaMarginY));
                        BrushInfo interior = GetInteriorWinLoss(Value,sparkline);
                        BrushPaint.FillRectangle(og, rect, interior);
                    }
                }
            }
        }
        /// <summary>
        /// Get interior BrushInfo for points
        /// </summary>
        private BrushInfo GetInteriorWinLoss(double curValue,ISparkLine sparkline)
        {
            BrushInfo interior = sparkline.ColumnStyle.ColumnColor;
            sparkline.Markers.ShowNegativePoint = true;
            if (m_sparklineSource.IsNegative)
            {
                int count = sparkline.GetNegativePoint().GetUpperBound(0);

                for (int k = 0; k <= count; k++)
                {
                    if (Convert.ToDouble(curValue) == (double)sparkline.GetNegativePoint().GetValue(k) && sparkline.Markers.ShowNegativePoint)
                    {
                        interior = sparkline.Markers.NegativePointColor;

                    }
                }
            }
            if (Convert.ToDouble(curValue) == sparkline.StartPoint && sparkline.Markers.ShowStartPoint)
                interior = sparkline.Markers.StartPointColor;
            if (Convert.ToDouble(curValue) == sparkline.EndPoint && sparkline.Markers.ShowEndPoint)
                interior = sparkline.Markers.EndPointColor;

            if (Convert.ToDouble(curValue) == sparkline.HighPoint &&sparkline.Markers.ShowHighPoint)
                interior = sparkline.Markers.HighPointColor;
            if (Convert.ToDouble(curValue) == sparkline.LowPoint && sparkline.Markers.ShowLowPoint)
                interior = sparkline.Markers.LowPointColor;
            return interior;

        }
    }
}
