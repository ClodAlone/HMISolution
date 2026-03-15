#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public interface ILexem : IBlock
    {
        /// <summary>
        ///
        /// </summary>
        IEnumerable SubLexems { get; set; }

        /// <summary>
        ///
        /// </summary>
        bool IsRegex { get; set; }

        /// <summary>
        ///
        /// </summary>
        string FormatName { get; set; }

        /// <summary>
        ///
        /// </summary>
        ScopeLevel ScopeLevel { get; set; }

        /// <summary>
        ///
        /// </summary>
        bool ShowAlternateIntellisenseText { get; set; }

        /// <summary>
        ///
        /// </summary>
        string IntellisenseDisplayText { get; set; }

        /// <summary>
        ///
        /// </summary>
        bool ExcludeItemInIntellisense { get; set; }

        /// <summary>
        ///
        /// </summary>
        bool Indent { get; set; }

        /// <summary>
        ///
        /// </summary>
        bool IsCollapsible { get; set; }

        /// <summary>
        ///
        /// </summary>
        bool EndBlockOnRecurrence { get; set; }
    }
}