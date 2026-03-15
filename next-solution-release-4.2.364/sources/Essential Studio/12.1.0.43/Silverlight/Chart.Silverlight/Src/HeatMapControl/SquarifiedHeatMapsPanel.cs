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
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Uses the squarified pattern to position the child items.
    /// </summary>
    public class SquarifiedHeatMapsPanel : HeatMapsPanel
    {
        /// <summary>
        /// Method used for computing ItemsBounds
        /// </summary>
        protected override void ComputeItemBounds()
        {
            this.Squarify(this.SortedItemMeasuresList, new List<HeatMapItemMeasure>(), this.ShorterSideLength);   
        }

        /// <summary>
        /// This method returns the Rectangle Bound
        /// </summary>
        /// <param name="orientation">Orientation is used to check.</param>
        /// <param name="item">item is used to set AreaByWeight</param>
        /// <param name="x">x is used to get x position.</param>
        /// <param name="y">y is used to get x position.</param>
        /// <param name="width">width is used to get x position.</param>
        /// <param name="height">height is used to get x position.</param>
        /// <returns>Type : Rect</returns>
        protected override Rect GetRectangle(Orientation orientation, HeatMapItemMeasure item, double x, double y, double width, double height)
        {
            if (orientation == Orientation.Horizontal)
            {
                return new Rect(x, y, width, item.AreaByWeight / width);
            }
            else
            {
                return new Rect(x, y, item.AreaByWeight / height, height);
            }
        }

        /// <summary>
        /// This method computes the Next Position
        /// </summary>
        protected override void ComputeNextPosition(Orientation orientation, ref double xPos, ref double yPos, double width, double height)
        {
            if (orientation == Orientation.Horizontal)
            {
                yPos += height;
            }
            else
            {
                xPos += width;
            }
        }

        private void Squarify(List<HeatMapItemMeasure> items, List<HeatMapItemMeasure> row, double sideLength)
        {
            if (items.Count == 0)
            {
                this.AddRowToLayout(row);
                return;
            }

            HeatMapItemMeasure item = items[0];
            List<HeatMapItemMeasure> row2 = new List<HeatMapItemMeasure>(row);
            row2.Add(item);
            List<HeatMapItemMeasure> items2 = new List<HeatMapItemMeasure>(items);
            items2.RemoveAt(0);

            double worst1 = this.Worst(row, sideLength);
            double worst2 = this.Worst(row2, sideLength);

            if (row.Count == 0 || worst1 > worst2)
            {
                this.Squarify(items2, row2, sideLength);
            }
            else
            {
                this.AddRowToLayout(row);
                this.Squarify(items, new List<HeatMapItemMeasure>(), this.ShorterSideLength);
            }
        }

        private void AddRowToLayout(List<HeatMapItemMeasure> row)
        {
            base.ComputeBySlicing(row);
        }

        private double Worst(List<HeatMapItemMeasure> row, double sideLength)
        {
            if (row.Count == 0)
            {
                return 0;
            }

            double maxArea = 0;
            double minArea = double.MaxValue;
            double totalArea = 0;
            foreach (HeatMapItemMeasure item in row)
            {
                maxArea = Math.Max(maxArea, item.AreaByWeight);
                minArea = Math.Min(minArea, item.AreaByWeight);
                totalArea += item.AreaByWeight;
            }

            if (minArea == double.MaxValue)
            {
                minArea = 0;
            }

            double val1 = (sideLength * sideLength * maxArea) / (totalArea * totalArea);
            double val2 = (totalArea * totalArea) / (sideLength * sideLength * minArea);
            return Math.Max(val1, val2);
        }
    }
}