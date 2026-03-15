#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Diagnostics;

namespace Syncfusion.Windows.Forms.Chart
{
    internal class ChartPointMinMax : IChartPointMinMax
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPointMinMax"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="xIndex">Index of the x.</param>
        public ChartPointMinMax(IChartSeriesModel model, int xIndex)
        {
            this.model = model;
            this.xIndex = xIndex;
        }

        /// <summary>
        /// Gets the Y.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="yIndex">Index of the y.</param>
        /// <returns>Returns the Y value.</returns>
        public double GetY(int index, int yIndex)
        {
            return this.model.GetY(index)[yIndex];
        }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        public double Max
        {
            get
            {
                if (this.model.GetEmpty(xIndex))
                {
                    return double.MinValue;
                }
                else
                {
                    return ChartMinMaxEvaluator.CalcMax(this.model.GetY(xIndex));
                }
            }
        }

        /// <summary>
        /// Gets the minimaum value.
        /// </summary>
        /// <value>The minimaum value.</value>
        public double Min
        {
            get
            {
                if (this.model.GetEmpty(xIndex))
                {
                    return double.MaxValue;
                }
                else
                {
                    return ChartMinMaxEvaluator.CalcMin(this.model.GetY(xIndex));
                }
            }
        }

        /// <summary>
        /// Gets the X Value.
        /// </summary>
        /// <value>The X Value.</value>
        public double X
        {
            get
            {
                return this.model.GetX(this.xIndex);
            }
        }

        private IChartSeriesModel model;
        private int xIndex;
    }
  
    internal class ChartMinMaxEvaluator
    {
        /// <summary>
        /// Calculates the minimum value.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>Returns the minimum value.</returns>
        public static double CalcMin(double[] values)
        {
            int count = values.GetLength(0);

            double min = Double.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (min > (double)values[i])
                {
                    min = values[i];
                }
            }

            return min;
        }

        /// <summary>
        /// Calculates  the maximum value.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>Returns the maximum value.</returns>
        public static double CalcMax(double[] values)
        {
            int count = values.GetLength(0);

            double max = Double.MinValue;

            for (int i = 0; i < count; i++)
            {
                if (max < values[i])
                {
                    max = values[i];
                }
            }

            return max;
        }
    }
}