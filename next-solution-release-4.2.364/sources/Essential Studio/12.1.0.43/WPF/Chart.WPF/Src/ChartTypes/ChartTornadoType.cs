// <copyright file="ChartTornadoType.cs" company="Syncfusion">
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
    /// Represents ChartTornadoType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartTornadoType : ChartRangeColumnType
    {
        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.Rotated | ChartTypeFlags.Indexed;
            }
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartTornadoType"/>
        public override string ToString()
        {
            return "Tornado";
        }
    }
}
