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
using System.Windows.Forms;
using System.ComponentModel;


namespace Syncfusion.Windows.Forms.Chart
{
   public class LineRender
    {
      
       public List<object> List = new List<object>();

       private int areaMarginX = 3;
       private int areaMarginY = 3;
       SparkLineSource m_sparklineSource=new SparkLineSource();

       public void LineSparkLine(Graphics og, ISparkLine sparkline)
        {

            List = (List<object>)m_sparklineSource.GetSourceList(sparkline.Source,sparkline);
            if (List.Count > 0)
            {
                double areaWidth = sparkline.ControlWidth - areaMarginX * areaMarginX;
                double areaHeight = sparkline.ControlHeight - areaMarginY * areaMarginY;

                double sInterval = areaWidth / (List.Count);
                double sRange = sparkline.HighPoint - sparkline.LowPoint;

                if (sRange > 0)
                {
                    double firstPointX = 0;
                    double firstPointY = 0;

                    double secondPointX = 0;
                    double secondPointY = 0;

                    for (int i = 0; i < List.Count; i++)
                    {
                        double Value = Convert.ToDouble(List[i]) - sparkline.LowPoint;

                        secondPointX = firstPointX;
                        secondPointY = firstPointY;

                        firstPointX = sInterval * i + (sInterval / 2);
                        firstPointY = areaHeight - (areaHeight * (Value / sRange));

                        if (i > 0)
                        {

                            og.DrawLine(new Pen(sparkline.LineStyle.LineColor, sparkline.LineStyle.LineWidth), (float)(areaMarginX + firstPointX), (float)(areaMarginY + firstPointY), (float)(areaMarginX + secondPointX), (float)(areaMarginY + secondPointY));

                        }

                        if (sparkline.Markers.ShowMarker)
                            og.FillEllipse(new SolidBrush(sparkline.Markers.MarkerColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);
                        if (Convert.ToDouble(List[i]) == sparkline.StartPoint && sparkline.Markers.ShowStartPoint)
                            og.FillEllipse(new SolidBrush(sparkline.Markers.StartPointColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);
                        if (Convert.ToDouble(List[i]) == sparkline.EndPoint && sparkline.Markers.ShowEndPoint)
                            og.FillEllipse(new SolidBrush(sparkline.Markers.EndPointColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);

                        if (sparkline.GetNegativePoint() != null)
                        {
                            int count = sparkline.GetNegativePoint().GetUpperBound(0);

                            for (int k = 0; k <= count; k++)
                            {
                                if (Convert.ToDouble(List[i]) == (double)sparkline.GetNegativePoint().GetValue(k) && sparkline.Markers.ShowNegativePoint)
                                {
                                    og.FillEllipse(new SolidBrush(sparkline.Markers.NegativePointColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);

                                }
                            }
                        }
                        if (Convert.ToDouble(List[i]) == sparkline.HighPoint && sparkline.Markers.ShowHighPoint)
                            og.FillEllipse(new SolidBrush(sparkline.Markers.HighPointColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);
                        if (Convert.ToDouble(List[i]) == sparkline.LowPoint && sparkline.Markers.ShowLowPoint)
                            og.FillEllipse(new SolidBrush(sparkline.Markers.LowPointColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);
                    }
                }
            }
            else
            {

                sparkline.Source = new double[] { 20, 90, 30, 60, 10 };
                List = (List<object>)(List<object>)m_sparklineSource.GetSourceList(sparkline.Source, sparkline);
                double areaWidth = sparkline.ControlWidth - areaMarginX * areaMarginX;
                double areaHeight = sparkline.ControlHeight - areaMarginY * areaMarginY;

                double sInterval = areaWidth / List.Count;
                double sRange = sparkline.HighPoint - sparkline.LowPoint;

                if (sRange > 0)
                {
                    double firstPointX = 0;
                    double firstPointY = 0;

                    double secondPointX = 0;
                    double secondPointY = 0;

                    for (int i = 0; i < List.Count; i++)
                    {
                        double Value = Convert.ToDouble(List[i]) - sparkline.LowPoint;

                        secondPointX = firstPointX;
                        secondPointY = firstPointY;

                        firstPointX = sInterval * i + (sInterval / 2);
                        firstPointY = areaHeight - (areaHeight * (Value / sRange));

                        if (i > 0)
                        {
                            og.DrawLine(new Pen(sparkline.LineStyle.LineColor, sparkline.LineStyle.LineWidth), (float)(areaMarginX + firstPointX), (float)(areaMarginY + firstPointY), (float)(areaMarginX + secondPointX), (float)(areaMarginY + secondPointY));

                        }

                               if (sparkline.Markers.ShowMarker)
                                    og.FillEllipse(new SolidBrush(sparkline.Markers.MarkerColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);
                                if (Convert.ToDouble(List[i]) == sparkline.StartPoint && sparkline.Markers.ShowStartPoint)
                                    og.FillEllipse(new SolidBrush(sparkline.Markers.StartPointColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);
                                if (Convert.ToDouble(List[i]) == sparkline.EndPoint &&sparkline. Markers.ShowEndPoint)
                                    og.FillEllipse(new SolidBrush(sparkline.Markers.EndPointColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);

                               if (sparkline.GetNegativePoint() != null)
                        {
                            int count = sparkline.GetNegativePoint().GetUpperBound(0);

                            for (int k = 0; k <= count; k++)
                            {
                                if (Convert.ToDouble(List[i]) == (double)sparkline.GetNegativePoint().GetValue(k) && sparkline.Markers.ShowNegativePoint)
                                {
                                    og.FillEllipse(new SolidBrush(sparkline.Markers.NegativePointColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);

                                }
                            }
                        }
                                if (Convert.ToDouble(List[i]) == sparkline.HighPoint && sparkline.Markers.ShowHighPoint)
                                    og.FillEllipse(new SolidBrush(sparkline.Markers.HighPointColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);
                                if (Convert.ToDouble(List[i]) == sparkline.LowPoint && sparkline.Markers.ShowLowPoint)
                                    og.FillEllipse(new SolidBrush(sparkline.Markers.LowPointColor.BackColor), (float)(areaMarginX + firstPointX - 2), (float)(areaMarginY + firstPointY - 2), 5, 5);

                            }
                        }
                    }
                }
            }
                         

        }
    

        