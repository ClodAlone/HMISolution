// <copyright file="ChartStackingBarType.cs" company="Syncfusion">
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

    /// <summary>
    /// Represents ChartStackingBarType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartStackingBarType : ChartStackingColumnType
    {
        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return base.Flags | ChartTypeFlags.Rotated;
            }
        }
        #endregion
        
        /// <summary>
        /// Converts ChartColumnType to string
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartStackingBarType"/>
        public override string ToString()
        {
            return "StackingBar";
        }
    }
}
