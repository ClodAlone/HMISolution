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
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
    /// <summary>
    /// The FullStackingArea Renderering class.
    /// </summary>
    internal class FullStackingAreaRenderer : StackingAreaRenderer
    {
        #region Properties
        /// <summary>
        /// Get description of regions.
        /// </summary>
        /// <value></value>
        protected override string RegionDescription
        {
            get
            {
                return "Full stacking area";
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FullStackingAreaRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public FullStackingAreaRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Public methods
        
        #endregion
    }
}