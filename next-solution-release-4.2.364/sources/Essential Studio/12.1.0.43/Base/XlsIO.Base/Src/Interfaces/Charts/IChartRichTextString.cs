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
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Represents a Rich Text String that can be used to apply several styles inside a single cell.
    /// </summary>
    public interface IChartRichTextString
      : IParentApplication
      , IOptimizedUpdate
    {
        #region Properties
        /// <summary>
        /// Returns the entire text value
        /// </summary>
        string Text { get; }
        /// <summary>
        /// Gets the formatting runs of rich-text
        /// </summary>
        ChartAlrunsRecord.TRuns[] FormattingRuns { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Sets font for range of characters.
        /// </summary>
        /// <param name="iStartPos">First character of the range.</param>
        /// <param name="iEndPos">Last character of the range.</param>
        /// <param name="font">Font to set.</param>
        void SetFont(int iStartPos, int iEndPos, IFont font);
        /// <summary>
        /// Gets font for the specified formatting run.
        /// </summary>
        /// <param name="tRuns">Formatting run to return its font</param>
        IFont GetFont(ChartAlrunsRecord.TRuns tRuns);
        #endregion
    }
}
