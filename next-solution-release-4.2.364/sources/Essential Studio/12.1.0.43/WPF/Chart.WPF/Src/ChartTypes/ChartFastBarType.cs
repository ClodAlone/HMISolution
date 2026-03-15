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

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents FastBarType for performance
    /// </summary>
    public sealed class ChartFastBarType : ChartFastColumnType
    {
        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.SideBySide | ChartTypeFlags.Rotated | ChartTypeFlags.Indexed;
            }
        }
        #endregion
        /// <summary>
        /// ChartBarType ToString method
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartBarType"/>
        public override string ToString()
        {
            return "FastBar";
        }
    }
}
