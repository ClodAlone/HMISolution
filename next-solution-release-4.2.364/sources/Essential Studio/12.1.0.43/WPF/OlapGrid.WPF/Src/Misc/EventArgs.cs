#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
namespace Syncfusion.Windows.Grid.Olap
#else
namespace Syncfusion.Silverlight.Grid.Olap 
#endif
{
    using System;
    using System.Windows;
    using Syncfusion.Olap.Engine;
    using System.Windows.Input;
    using Syncfusion.Windows.Controls.Grid;
#if SILVERLIGHT
    using Syncfusion.OlapSilverlight.Engine;
#endif


    /// <summary>
    /// OlapGridDrillDownEventArgs
    /// </summary>
    public class OlapGridDrillDownEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridDrillDownEventArgs"/> class.
        /// </summary>
        /// <param name="gridArea">The grid area.</param>
        public OlapGridDrillDownEventArgs(UIElement gridArea)
            : this()
        {
            this.GridArea = gridArea;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridDrillDownEventArgs"/> class.
        /// </summary>
        public OlapGridDrillDownEventArgs()
        {
            this.ShowDefaultIndicator = true;
        }

        /// <summary>
        /// Gets or sets the waiting dialog.
        /// </summary>
        /// <value>The waiting dialog.</value>
        public UIElement GridArea
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show default indicator].
        /// </summary>
        /// <value>
        /// <c>true</c> if [a default waiting dialog will be displayed on drill down]; user can customize the dialog, <c>false</c>.
        /// </value>
        public bool ShowDefaultIndicator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cell descriptor.
        /// </summary>
        /// <value>The cell descriptor.</value>
        public PivotCellDescriptor CellDescriptor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current cursor.
        /// </summary>
        /// <value>The current cursor.</value>
        internal Cursor CurrentCursor
        {
            get;
            set;
        }
    }

    public class LinkLabelEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkLabelEventArgs"/> class.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        public LinkLabelEventArgs(PivotCellDescriptor cellDescriptor)
        {
            this.CellDescriptor = cellDescriptor;
        }

        /// <summary>
        /// Gets or sets the cell descriptor.
        /// </summary>
        /// <value>The cell descriptor.</value>
        public PivotCellDescriptor CellDescriptor
        {
            get;
            set;
        }
    }

    public class OlapGridSelectionChangedEventArgs : EventArgs
    {
        #region [ Private Variables ]
        private Syncfusion.Windows.Controls.Grid.GridRangeInfo m_CellRangeInfo;
        private SelectedItems m_SelectedItems;
        private GridSelectionReason m_SelectionReason;
        #endregion

        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridSelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="RangeInfo">The range info.</param>
        public OlapGridSelectionChangedEventArgs(Syncfusion.Windows.Controls.Grid.GridRangeInfo RangeInfo, SelectedItems SelectedItems, GridSelectionReason SelectionReason)
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
        public Syncfusion.Windows.Controls.Grid.GridRangeInfo CellRangeInfo
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
}
