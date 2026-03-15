#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
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
    public class ApplyExpandCollapseArgs
    {
        /// <summary>
        ///
        /// </summary>
        public LineItemExpandInformation ExpandInformation { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public List<BlockListener> LanguageBlocks { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public List<LineItemExpandInformation> Source { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public List<Uri> Assemblies { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<ILexem> Lexems { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<IFormat> Formats { get; internal set; }
    }
}