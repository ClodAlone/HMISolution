//-------------------------------------------------------------------------------------------------
// <copyright file="CustomEngine.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;   
    using System.Collections; 
    using System.ComponentModel;
    using System.Drawing;    
    using System.Data;
    using System.Diagnostics; 
    using System.Data.OleDb;
    using System.IO;
    using System.Text;    
    using System.Windows.Forms;

    using Syncfusion.Grouping;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Grid.Grouping;

    /// <summary>
    /// GroupingEngineFactory provides GroupingRecordRow elements that
    /// support saving row heights.
    /// </summary>
    public class AllowResizingIndividualRows : GridEngineFactoryBase
    {
        // Add this line in your forms ctor:
        // GridEngineFactory.Factory = new GroupingEngineFactory();

        /// <summary>
        /// Creates a new grouping engine to provide a modified GridChildTable that adds an extra section.
        /// </summary>
        /// <returns>The new grouping engine.</returns>
        public override GridEngine CreateEngine()
        {
            return new GroupingEngine();
        }
    }

    /// <summary>
    /// Defines a custom engine for grouping grid.
    /// </summary>
    public class GroupingEngine : GridEngine
    {
        /// <summary>
        /// Initializes a new <see cref="GroupingEngine"/>
        /// </summary>
        public GroupingEngine()
            : base()
        {
        }

        /// <summary>
        /// Creates a new record row.
        /// </summary>
        /// <param name="parent">Parent record row.</param>
        /// <returns>The new record row.</returns>
        public override RecordRow CreateRecordRow(RecordRowsPart parent)
        {
            return new GroupingRecordRow(parent);
        }

        /// <summary>
        /// Creates a new caption row.
        /// </summary>
        /// <param name="parent">Parent caption.</param>
        /// <returns>The new caption row.</returns>
        public override CaptionRow CreateCaptionRow(CaptionSection parent)
        {
            return new GroupingCaptionRow(parent);
        }

        /// <summary>
        /// Creates a new column header row.
        /// </summary>
        /// <param name="parent">Parent column header section.</param>
        /// <returns>The new column header row.</returns>
        public override ColumnHeaderRow CreateColumnHeaderRow(ColumnHeaderSection parent)
        {
            return new GroupingColumnHeaderRow(parent);
        }

        // same pattern can be used for:
        // FilterBarRow CreateFilterBarRow(FilterBarSection parent)
        // GroupFooterSection CreateGroupFooterSection(Group parent)
        // GroupHeaderSection CreateGroupHeaderSection(Group parent)
        // RecordPreviewRow CreateRecordPreviewRow(RecordPreviewRowsPart parent)
    }

    /// <summary>
    /// Defines a custom grid record row.
    /// </summary>
    public class GroupingRecordRow : GridRecordRow, IGridRowHeight
    {
        int rowHeight = -1;

        /// <summary>
        /// Initializes a new object in the specifed record part.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        public GroupingRecordRow(RecordRowsPart parent)
            : base(parent)
        {
        }
        #region IGridRowHeight Members

        /// <summary>
        /// Determines if elements supports storing row heights
        /// </summary>
        /// <returns>True if it supports; False otherwise.</returns>
        public bool SupportsRowHeight()
        {
            return true;
        }

        /// <summary>
        /// Gets or sets the row height.
        /// </summary>
        public int RowHeight
        {
            get
            {
                return this.rowHeight;
            }

            set
            {
                if (this.rowHeight != value)
                {
                    this.rowHeight = value;
                    this.InvalidateCounterBottomUp();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the row height was modified or if default setting should be used.
        /// </summary>
        public bool HasRowHeight
        {
            get
            {
                return this.rowHeight != -1;
            }
        }

        #endregion

        /// <summary>
        /// This is where the row height then gets integrated with the engine
        /// YAmount Counter logic.
        /// </summary>
        /// <returns>Element height.</returns>
        public override double GetYAmountCount()
        {
            // Note: whenever the value that is returned by GetYAmountCount changes
            // make sure you call InvalidateCounterBottomUp so that the engine
            // is aware of the change and counters are recalculated. See
            // the RowHeight setter.
            return this.rowHeight != -1 ? this.rowHeight : base.GetYAmountCount();
        }
    }

    /// <summary>
    /// Defines a custom group caption.
    /// </summary>
    public class GroupingCaptionRow : GridCaptionRow, IGridRowHeight
    {
        int rowHeight = -1;

        /// <summary>
        /// Initializes a new object in the specifed record part.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        public GroupingCaptionRow(CaptionSection parent)
            : base(parent)
        {
        }
        #region IGridRowHeight Members

        /// <summary>
        /// Determines if elements supports storing row heights
        /// </summary>
        /// <returns>True if it supports; False otherwise.</returns>
        public bool SupportsRowHeight()
        {
            return true;
        }

        /// <summary>
        /// Gets or sets the row height
        /// </summary>    
        public int RowHeight
        {
            get
            {
                return this.rowHeight;
            }

            set
            {
                if (this.rowHeight != value)
                {
                    this.rowHeight = value;
                    this.InvalidateCounterBottomUp();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether row height was modified or if default setting should be used.
        /// </summary>
        public bool HasRowHeight
        {
            get
            {
                return this.rowHeight != -1;
            }
        }

        #endregion

        /// <summary>
        /// This is where the row height then gets integrated with the engine
        /// YAmount Counter logic.
        /// </summary>
        /// <returns>Element height.</returns>
        public override double GetYAmountCount()
        {
            // Note: whenever the value that is returned by GetYAmountCount changes
            // make sure you call InvalidateCounterBottomUp so that the engine
            // is aware of the change and counters are recalculated. See
            // the RowHeight setter.
            return this.rowHeight != -1 ? this.rowHeight : base.GetYAmountCount();
        }
    }

    /// <summary>
    /// Defines a custom column header row.
    /// </summary>
    public class GroupingColumnHeaderRow : GridColumnHeaderRow, IGridRowHeight
    {
        int rowHeight = -1;

        /// <summary>
        /// Initializes a new object in the specifed record part.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        public GroupingColumnHeaderRow(ColumnHeaderSection parent)
            : base(parent)
        {
        }
        #region IGridRowHeight Members

        /// <summary>
        /// Determines if elements supports storing row heights
        /// </summary>
        /// <returns>True if it supports; False otherwise.</returns>
        public bool SupportsRowHeight()
        {
            return true;
        }

        /// <summary>
        /// Gets or sets the row height
        /// </summary>
        public int RowHeight
        {
            get
            {
                return this.rowHeight;
            }

            set
            {
                if (this.rowHeight != value)
                {
                    this.rowHeight = value;
                    this.InvalidateCounterBottomUp();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the row height was modified or if default setting should be used.
        /// </summary>
        public bool HasRowHeight
        {
            get
            {
                return this.rowHeight != -1;
            }
        }

        #endregion

        /// <summary>
        /// This is where the row height then gets integrated with the engine
        /// YAmount Counter logic.
        /// </summary>
        /// <returns>Row height.</returns>
        public override double GetYAmountCount()
        {
            // Note: whenever the value that is returned by GetYAmountCount changes
            // make sure you call InvalidateCounterBottomUp so that the engine
            // is aware of the change and counters are recalculated. See
            // the RowHeight setter.
            return this.rowHeight != -1 ? this.rowHeight : base.GetYAmountCount();
        }
    }
}
