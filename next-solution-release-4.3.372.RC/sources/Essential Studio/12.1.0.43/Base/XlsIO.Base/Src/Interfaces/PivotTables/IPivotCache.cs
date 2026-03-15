#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Represents pivot cache.
    /// </summary>
    public interface IPivotCache
    {
        /// <summary>
        /// Gets zero-based cache index.Read-only.
        /// </summary>
        int Index { get; }
        /// <summary>
        /// Specifies the pivot table cache source type.Read-only.
        /// </summary>
        ExcelDataSourceType SourceType { get; }
        /// <summary>
        /// Returns the data source for the PivotTable report. Read-only.
        /// </summary>
        IRange SourceRange { get; set; }
    }
}
