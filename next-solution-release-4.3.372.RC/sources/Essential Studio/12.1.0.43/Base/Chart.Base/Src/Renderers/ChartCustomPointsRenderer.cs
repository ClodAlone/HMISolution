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

using System.Diagnostics;
using System.Drawing;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The ChartCustomPoints Renderering class.
    /// </summary>
    internal class ChartCustomPointsRenderer : ChartSeriesRenderer
    {
        #region Properties
        /// <summary>
        /// Gets count of require Y values of the points.
        /// </summary>
        /// <value></value>
        protected override int RequireYValuesCount
        {
            get
            {
                return 1;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCustomPointsRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public ChartCustomPointsRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
        /// the rendering.
        /// </summary>
        /// <param name="g">The graphics object that is to be used for rendering.</param>
        public override void Render(Graphics g)
        {
            base.RenderAdornments(g);
        }

        /// <summary>
        /// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
        /// the rendering.
        /// </summary>
        /// <param name="g">The graphics object that is to be used for rendering.</param>
        public override void Render(Graphics3D g)
        {
            base.RenderAdornments(g);
        }
        #endregion
    }
}