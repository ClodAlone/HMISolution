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
   public class ColumnRender
    {
        public List<object> List = new List<object>();

        private int areaMarginX = 3;
        private int areaMarginY = 3;
        SparkLineSource m_sparklineSource = new SparkLineSource();

         //<summary>
         //Renderer the Column type series
         //</summary>
         //<param name="og">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        public void DrawSparkColumn(Graphics og,ISparkLine sparkline)
        {
            List = (List<object>)m_sparklineSource.GetSourceList(sparkline.Source, sparkline);
            
            if (List.Count > 0)
            {
                double areaWidth = sparkline.ControlWidth  - areaMarginX * areaMarginX;
                double areaHeight = sparkline.ControlHeight - areaMarginY * areaMarginX;

                double firstPointX = 0;
                double firstPointY = 0;
                double sRange = 0;

                double sInterval = areaWidth / (List.Count);
                
                if (!m_sparklineSource.IsNegative)
                {
                    sRange =sparkline.HighPoint - 0;

                    if (sRange > 0)
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            double Value = Convert.ToDouble(List[i]);

                            firstPointX = sInterval * i + (sInterval / 2);

                            firstPointY = (areaHeight * (Value / sRange));

                            if ((int)firstPointY == 0) firstPointY = 1;

                            Rectangle rect = new Rectangle((int)(areaMarginX + firstPointX - sInterval / 2 + sparkline.ColumnStyle.ColumnSpace), (int)(areaMarginY + (areaHeight - firstPointY)), (int)(sInterval -sparkline.ColumnStyle.ColumnSpace * 2), (int)(firstPointY));

                            BrushInfo interior = GetInteriorColumn(Value,sparkline);
                            BrushPaint.FillRectangle(og, rect, interior);
                        }
                    }
                }
                else
                {
                    sRange = sparkline.HighPoint - sparkline.LowPoint;
                    if (sRange > 0)
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            double ValueWidth = Convert.ToDouble(List[i]);
                            double ValueYRect = Convert.ToDouble(List[i]) - (sparkline.LowPoint);

                            firstPointX = sInterval * i + (sInterval / 2);

                            firstPointY = areaHeight - (areaHeight * (ValueYRect / sRange));

                            double rectWidth = (sInterval - sparkline.ColumnStyle.ColumnSpace * 2);

                            double rectHeight = (areaHeight * (ValueWidth / sRange));
                            if (Convert.ToDouble(List[i]) < 0)
                            {
                                firstPointY = firstPointY - Math.Abs(rectHeight);
                            }
                            if ((int)rectHeight == 0) rectHeight = 1;

                            Rectangle rect = new Rectangle((int)(areaMarginX + firstPointX - sInterval / 2 + sparkline.ColumnStyle.ColumnSpace), (int)(areaMarginY + (firstPointY)), (int)(rectWidth), (int)Math.Abs(rectHeight));
                            BrushInfo interior = GetInteriorColumn(ValueWidth, sparkline);
                            BrushPaint.FillRectangle(og, rect, interior);

                        }
                    }
                }
            }
        }
        //<summary>
        //Get interior BrushInfo for points
        //</summary>
        private BrushInfo GetInteriorColumn(double curValue,ISparkLine sparkline)
        {
            BrushInfo interior = sparkline.ColumnStyle.ColumnColor;

            if (Convert.ToDouble(curValue) == sparkline.StartPoint && sparkline.Markers.ShowStartPoint)
                interior = sparkline.Markers.StartPointColor;
            if (Convert.ToDouble(curValue) == sparkline.EndPoint && sparkline.Markers.ShowEndPoint)
                interior = sparkline.Markers.EndPointColor;

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
            if (Convert.ToDouble(curValue) == sparkline.HighPoint &&sparkline.Markers.ShowHighPoint)
                interior = sparkline.Markers.HighPointColor;
            if (Convert.ToDouble(curValue) == sparkline.LowPoint && sparkline.Markers.ShowLowPoint)
                interior = sparkline.Markers.LowPointColor;
            return interior;

        }
    }
}
