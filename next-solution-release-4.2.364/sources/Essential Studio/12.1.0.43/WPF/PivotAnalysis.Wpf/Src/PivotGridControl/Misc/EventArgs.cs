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

#if !SILVERLIGHT
using Syncfusion.Windows.Controls.Grid;
using System.ComponentModel;
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Styles;
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.Windows.Controls.Grid;
using System.ComponentModel;
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Cells;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    ///  A class containing event data related to during data refreshing in PivotGridControl
    /// </summary>
    public class DataRefreshingArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes the <see cref="DataRefreshingArgs"/> class.
        /// </summary>
        public DataRefreshingArgs()
        {

        }
    }
    /// <summary>
    ///  A class containing event data related to after data refreshed in PivotGridControl
    /// </summary>
         
    public class DataRefreshedArgs : EventArgs
    {
        /// <summary>
        /// Initializes the <see cref="DataRefreshedArgs"/> class.
        /// </summary>
        public DataRefreshedArgs()
        {

        }
    }
    /// <summary>
    /// SchemaDesigner's Fieldlist should be reinitialized whenever the itemsource changed in PivotGrid
    /// </summary>
         
    public class ItemsSourceChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the <see cref="ItemsSourceChangedEventArgs"/> class.
        /// </summary>
        public ItemsSourceChangedEventArgs()
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

    /// <summary>
    /// A class containing event data related to after selection change in PivotGridControl.
    /// </summary>
    public class PivotGridSelectionChangedEventArgs : EventArgs
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
        /// <param name="SelectedItems">The collection of selected items</param>
        /// <param name="SelectionReason">The current state of user action and reason for the event</param>
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

    /// <summary>
    /// A class containing event data related to before expand operation in PivotGridControl.
    /// </summary>
    public class ExpandingEventArgs : CancelEventArgs
    {
        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandingEventArgs"/> class.
        /// </summary>
       /// <param name="PivotCellInfo">The pivot cell info</param>
        public ExpandingEventArgs(PivotCellInfo PivotCellInfo)
        {
            this.PivotCellInfo = PivotCellInfo;            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandingEventArgs"/> class.
        /// </summary>
        /// <param name="PivotCellInfo">The pivot cell info</param>
        /// <param name="uniqueText">The unique text</param>
        /// <param name="rowColumnIndex">The row and column index</param>
        public ExpandingEventArgs(PivotCellInfo PivotCellInfo, string uniqueText,  RowColumnIndex rowColumnIndex)
        {
            this.PivotCellInfo = PivotCellInfo;
            this.UniqueText = uniqueText;
        }
        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the PivotCellDescriptor
        /// </summary>
        /// <value>The pivot cell info.</value>
        public PivotCellInfo PivotCellInfo { get; set; }
        /// <summary>
        /// Gets or sets the UniqueText 
        /// </summary>
        public string UniqueText { get; set; }

        #endregion
    }

    /// <summary>
    /// A class containing event data related to after expand operation in PivotGridControl.
    /// </summary>
    public class ExpandedEventArgs : EventArgs
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

    /// <summary>
    /// A class containing event data related to before collapse operation in PivotGridControl.
    /// </summary>
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

    /// <summary>
    /// A class containing event data related to after collapse operation in PivotGridControl.
    /// </summary>
    public class CollapsedEventArgs : EventArgs
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

    /// <summary>
    /// A class containing event data related to click operation in hyperlink cell in PivotGridControl.
    /// </summary>
    public class HyperlinkCellClickEventArgs : EventArgs
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperlinkCellClickEventArgs"/> class.
        /// </summary>
        /// <param name="PivotCellInfo">The pivot cell info.</param>
        /// <param name="cellRowColumnIndex">The RowColumnIndex of the cell.</param>
        /// <param name="e">The MouseButtonEventArgs.</param>
        public HyperlinkCellClickEventArgs(PivotCellInfo PivotCellInfo, RowColumnIndex cellRowColumnIndex, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.PivotCellInfo = PivotCellInfo;
            this.RowColumnIndex = cellRowColumnIndex;
            this.MouseButtonArgs = e;
        }
        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the PivotCellDescriptor
        /// </summary>
        /// <value>The pivot cell info.</value>
        public PivotCellInfo PivotCellInfo { get; set; }

        /// <summary>
        /// Gets the RowColumnIndex of the clicked cell
        /// </summary>
        /// <value>The RowColumnIndex value.</value>
        public RowColumnIndex RowColumnIndex { get; internal set; }

        /// <summary>
        /// Gets the MouseButtonEventArgs of the clicked cell
        /// </summary>
        /// <value>The MouseButtonEventArgs value.</value>
        public System.Windows.Input.MouseButtonEventArgs MouseButtonArgs { get; internal set; }
        #endregion
    }

#if SILVERLIGHT
    public class CommandEventArgs : EventArgs
    {
        public object Item { get; set; }

        public bool IsSort { get; set; }

        public object Tag { get; set; }

        public CommandEventArgs(object item, bool sort, object tag)
        {
            this.Item = item;
            this.IsSort = sort;
            this.Tag = tag;
        }
    }

    public class ArrangeOverrideEventArgs :EventArgs
    {
        public ListBoxItem ListBoxItem { get; set; }

        public ArrangeOverrideEventArgs(ListBoxItem listBoxItem)
        {
            this.ListBoxItem = listBoxItem;
        }
    }

    public class ItemContainerPrepared : EventArgs
    {
        public ListBoxItem ListBoxItem { get; set; }

        public ItemContainerPrepared(ListBoxItem listBoxItem)
        {
            this.ListBoxItem = listBoxItem;
        }
    }

#endif
}
