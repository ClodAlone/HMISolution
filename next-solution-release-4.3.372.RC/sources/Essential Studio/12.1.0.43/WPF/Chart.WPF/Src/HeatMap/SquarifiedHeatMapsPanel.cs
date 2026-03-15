// <copyright file="SquarifiedHeatMapsPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Uses the squarified pattern to position the child items.
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class SquarifiedHeatMapsPanel : HeatMapsPanel
    {
        /// <summary>
        /// Computes Item Bounds
        /// </summary>
        protected override void ComputeItemBounds()
        {
            this.Squarify(this.SortedItemMeasuresList, new List<HeatMapItemMeasure>(), this.ShorterSideLength);
        }

        /// <summary>
        /// Method to Get Rectangle
        /// </summary>
        /// <param name="orientation">The orientation</param>
        /// <param name="item">The item value.</param>
        /// <param name="x">The x value</param>
        /// <param name="y">The Y value</param>
        /// <param name="width">The Width value</param>
        /// <param name="height">The height value</param>
        /// <returns>The Rectangle</returns>
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
        /// Method to Compute Next Position
        /// </summary>
        /// <param name="orientation">The orientation</param>
        /// <param name="xPos">The X position</param>
        /// <param name="yPos">The Y position</param>
        /// <param name="width">The Width value</param>        
        /// <param name="height">The height value</param>
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

        /// <summary>
        /// The Squarify method
        /// </summary>
        /// <param name="items">The items value</param>
        /// <param name="row">The row value</param>
        /// <param name="sideLength">The side length</param>
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

        /// <summary>
        /// The AddRowToLayout method
        /// </summary>
        /// <param name="row">The HeatMapItemMeasure row</param>
        /// <remarks></remarks>
        /// <seealso cref="SquarifiedHeatMapsPanel"/>
        private void AddRowToLayout(List<HeatMapItemMeasure> row)
        {
            base.ComputeBySlicing(row);
        }

        /// <summary>
        /// The Worst value method
        /// </summary>
        /// <param name="row">The HeatMapItemMeasure row</param>
        /// <param name="sideLength">The side length</param>
        /// <returns>The double value</returns>      
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
