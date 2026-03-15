
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


#region file using directives
using System;
#if ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO
{
    public interface IThreeDFormat
    {
        #region Interface properties
        /// <summary>
        /// Gets or sets the bevel top.
        /// </summary>
        /// <value>The bevel top.</value>
        Excel2007ChartBevelProperties BevelTop { get; set; }


        /// <summary>
        /// Gets or sets the bevel bottom.
        /// </summary>
        /// <value>The bevel bottom.</value>
        Excel2007ChartBevelProperties BevelBottom { get; set; }


        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        /// <value>The material.</value>
        Excel2007ChartMaterialProperties Material { get; set; }


        /// <summary>
        /// Gets or sets the lighting.
        /// </summary>
        /// <value>The lighting.</value>
        Excel2007ChartLightingProperties Lighting { get; set; }
        #endregion
    }
}
