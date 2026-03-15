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

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// ChartMarker is used in association with <see cref="ChartSymbolInfo"/>.
    /// </summary>
    /// <seealso cref="ChartSymbolInfo"/>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChartMarker
    {
        #region Members
        private LineCap lineCap = LineCap.Flat;
        private ChartLineInfo lineInfo = ChartLineInfo.CreateDefault();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the line cap that is to be used with this marker.
        /// </summary>
        public LineCap LineCap
        {
            get
            {
                return lineCap;
            }

            set
            {
                lineCap = value;
            }
        }

        /// <summary>
        /// Gets or sets the line information associated with this marker.
        /// </summary>
        public ChartLineInfo LineInfo
        {
            get
            {
                return lineInfo;
            }

            set
            {
                lineInfo = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartMarker"/> class.
        /// </summary>
        public ChartMarker()
        {
            lineInfo.Width = 5f;
        }
        #endregion
    }
}