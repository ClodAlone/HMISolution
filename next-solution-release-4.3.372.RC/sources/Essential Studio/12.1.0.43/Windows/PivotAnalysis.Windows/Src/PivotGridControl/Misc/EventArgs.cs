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
using Syncfusion.Windows.Forms.Grid;
using System.ComponentModel;
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    public class ItemSourceChangedEventArgs : SyncfusionEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSourceChangedEventArgs"/> class.
        /// </summary>
        public ItemSourceChangedEventArgs()
        {
        }
        /// <summary>
        /// Gets or sets the OldValue.
        /// </summary>
        public object OldValue { get; set; }
        /// <summary>
        /// Gets or sets the NewValue.
        /// </summary>
        public object NewValue { get; set; }

    }

    public class DataRefreshingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRefreshingArgs"/> class.
        /// </summary>
        public DataRefreshingEventArgs()
        {

        }
    }

    public class DataRefreshedEventArgs : SyncfusionEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRefreshedArgs"/> class.
        /// </summary>
        public DataRefreshedEventArgs()
        {

        }
    }

    public class PivotGridSelectionChangedEventArgs : SyncfusionEventArgs
    {
        #region [ Private Variables ]
        private GridRangeInfo m_CellRangeInfo;
        private SelectedItems m_SelectedItems;
        private GridSelectionReason m_SelectionReason;
        #endregion

        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="PivotGridSelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="RangeInfo">The range info.</param>
        public PivotGridSelectionChangedEventArgs(GridRangeInfo RangeInfo, SelectedItems SelectedItems, GridSelectionReason SelectionReason)
        {
            this.m_CellRangeInfo = RangeInfo;
            this.m_SelectedItems = SelectedItems;
            this.SelectionReason = SelectionReason;
        }
        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the Cell Range Information.
        /// </summary>
        /// <value>The Cell range info.</value>
        public GridRangeInfo CellRangeInfo
        {
            get
            {
                return m_CellRangeInfo;
            }
            internal set
            {
                m_CellRangeInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets the Selected items in the IEnumerable form of Columns, Rows and Value.
        /// </summary>
        /// <value>The selected items.</value>
        public SelectedItems SelectedItems
        {
            get
            {
                return m_SelectedItems;
            }
            internal set
            {
                m_SelectedItems = value;
            }
        }

        /// <summary>
        /// Gets or sets the Selection reason.
        /// </summary>
        /// <value>The selection reason.</value>
        public GridSelectionReason SelectionReason
        {
            get
            {
                return m_SelectionReason;
            }
            internal set
            {
                m_SelectionReason = value;
            }
        }
        #endregion
    }

    public class ExpandingEventArgs : CancelEventArgs
    {
        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandingEventArgs"/> class.
        /// </summary>
        public ExpandingEventArgs(PivotCellInfo PivotCellInfo)
        {
            this.PivotCellInfo = PivotCellInfo;
        }
        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the PivotCellDescriptor
        /// </summary>
        /// <value>The pivot cell info.</value>
        public PivotCellInfo PivotCellInfo { get; set; }
        #endregion
    }

    public class ExpandedEventArgs : SyncfusionEventArgs
    {
        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandedEventArgs"/> class.
        /// </summary>
        /// <param name="PivotCellInfo">The pivot cell info.</param>
        public ExpandedEventArgs(PivotCellInfo PivotCellInfo)
        {
            this.PivotCellInfo = PivotCellInfo;
        }
        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the PivotCellDescriptor
        /// </summary>
        /// <value>The pivot cell info.</value>
        public PivotCellInfo PivotCellInfo { get; set; }
        #endregion
    }

    public class CollapsingEventArgs : CancelEventArgs
    {
        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="CollapsingEventArgs"/> class.
        /// </summary>
        public CollapsingEventArgs(PivotCellInfo PivotCellInfo)
        {
            this.PivotCellInfo = PivotCellInfo;
        }
        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the PivotCellDescriptor
        /// </summary>
        /// <value>The pivot cell info.</value>
        public PivotCellInfo PivotCellInfo { get; set; }
        #endregion
    }

    public class CollapsedEventArgs : SyncfusionEventArgs
    {
        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="CollapsedEventArgs"/> class.
        /// </summary>
        /// <param name="PivotCellInfo">The pivot cell info.</param>
        public CollapsedEventArgs(PivotCellInfo PivotCellInfo)
        {
            this.PivotCellInfo = PivotCellInfo;
        }
        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the PivotCellDescriptor
        /// </summary>
        /// <value>The pivot cell info.</value>
        public PivotCellInfo PivotCellInfo { get; set; }
        #endregion
    }

    public class HyperlinkCellClickEventArgs : SyncfusionEventArgs
    {
        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperlinkCellClickEventArgs"/> class.
        /// </summary>
        /// <param name="PivotCellInfo">The pivot cell info.</param>
        public HyperlinkCellClickEventArgs(PivotCellInfo PivotCellInfo)
        {
            this.PivotCellInfo = PivotCellInfo;
        }
        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the PivotCellDescriptor
        /// </summary>
        /// <value>The pivot cell info.</value>
        public PivotCellInfo PivotCellInfo { get; set; }
        #endregion
    }


}
