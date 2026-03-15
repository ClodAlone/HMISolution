#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Windows;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class LineItemExpandInformation
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Text in a line
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the line is expanded or not
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool IsExpanded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether ContainsLines contain child item.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool ContainsLines
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the StartLine
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int StartLine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the EndLine
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int EndLine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the line is selected.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool IsSelected
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string PreprocessorText
        {
            get;
            set;
        }

        internal Point EllipsisPosition
        {
            get;
            set;
        }

        internal BlockListener LineStartBlockListener
        {
            get;
            set;
        }

        internal Stack<BlockListener> ParentListeners
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ParentLineNumber
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int ParentLineNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the LineStartBlock
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.BlockCodes
        /// </value>
        public BlockCodes LineStartBlock
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public IFormat LineStartFormat
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool ContainsPreprocessor
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool ToggleExpansion
        {
            get;
            set;
        }

        #endregion Properties
    }
}