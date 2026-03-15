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
using Syncfusion.XlsIO.Implementation.Collections;
namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Represents the conditional formatting defined in the PivotTable.
    /// </summary>
    class PivotConditionalFormat
    {
        #region Members
        /// <summary>
        /// Specifies the priority of PivotTable conditional formatting rule
        /// </summary>
        private int m_iPriority;
        /// <summary>
        /// Specifies the scope of PivotTable conditional formatting rule.
        /// </summary>
        private ConditionalFormatScope scope;
        /// <summary>
        /// This simple type defines the values for the Top N conditional formatting evaluation for the PivotTable
        /// </summary>
        private ConditionalTopNType m_formatType;
        /// <summary>
        /// Area in which the conditional format applied
        /// </summary>
        private PivotAreaCollection m_pivotAreas;
        #endregion
    }
}
