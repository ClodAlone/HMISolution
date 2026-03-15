#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        ///
        /// </summary>
        public IEnumerable<LineItem> MatchingLineItem { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string CaptureText { get; set; }

        /// <summary>
        ///
        /// </summary>
        public LineItem CurrentLineItem { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int CursorIndex { get; set; }

        /// <summary>
        ///
        /// </summary>
        public SelectionPointer CurrentSelection { get; set; }

        /// <summary>
        ///
        /// </summary>
        public SelectionPointer EditTextSelection { get; set; }

        internal bool IsDirty { get; set; }

        internal bool IsTextFound { get; set; }

        internal bool IsSelectionChanged { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<FindResult> FindAllResult { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public SearchResult()
        {
            IsDirty = false;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class FindResult
    {
        /// <summary>
        ///
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Index { get; set; }
    }
}