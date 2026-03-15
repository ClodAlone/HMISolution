#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    /// Popup for computation columns
    /// </summary>
    public class ColumnFilterPopup : Popup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.PivotGrid.ColumnFilterPopup">ColumnFilterPopup</see> class. 
        /// </summary>
        public ColumnFilterPopup()
        {
        }

        internal bool CanExecute { get; set; }
       
        internal List<FilterChoice> CancelCollectionList { get; set; }        

        private int colIndex;
        /// <summary>
        /// Gets or Sets the column index
        /// </summary>
        public int ColIndex
        {
            get { return colIndex; }
            set { colIndex = value; }
        }

        private bool _isFilterChanged = false;

        internal bool IsFilterChanged
        {
            get { return _isFilterChanged;}
            set { _isFilterChanged = value; }
        }
        /// <summary>
        /// sets the Items of Filter Popup
        /// </summary>
        public static Dictionary<string, List<FilterChoice>> FilterPopUpCollection = new Dictionary<string, List<FilterChoice>>();

        internal static List<string> FilteredColumnlist = new List<string>();
        //exclusions maps column names to a set of filter exclusions for that column. For value columns, the key
        //is just the columnName
        internal static Dictionary<string, HashSet<string>> Exclusions = new Dictionary<string, HashSet<string>>();

        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            if (!IsFilterChanged)
                return;
            CanExecute = true;
        }

        /// <summary>
        /// Gets or sets the filter list box.
        /// </summary>
        /// <value>The filter list box.</value>
        public ListBox FilterListBox { get; set; }

        private Dictionary<object, bool>.KeyCollection m_FilterItemsCollection;
        /// <summary>
        /// Gets or sets the filter list.
        /// </summary>
        /// <value>The filter list.</value>
        public Dictionary<object, bool>.KeyCollection ColFilterList
        {
            get
            {
                return m_FilterItemsCollection;
            }
            set
            {
                m_FilterItemsCollection = value;
                this.FilterListBox.ItemsSource = m_FilterItemsCollection;
            }
        }

        private Button m_ButtonApply;
        private Button m_ButtonCancel;

        /// <summary>
        /// Gets or sets the Apply Button.
        /// </summary>
        internal Button OkButton
        {
            get
            {
                return m_ButtonApply;
            }
            set
            {
                m_ButtonApply = value;
                WireButtonApplyEvent();
            }
        }

        /// <summary>
        /// Gets or sets the Cancel Button.
        /// </summary>
        internal Button CancelButton
        {
            get
            {
                return m_ButtonCancel;
            }
            set
            {
                m_ButtonCancel = value;
                WireButtonCancelEvent();
            }
        }

        private Thumb rightCornerThumb;

        internal Thumb RightCornerThumb
        {
            get { return rightCornerThumb; }
            set
            {
                if (value != null)
                {
                    rightCornerThumb = value;
                    rightCornerThumb.DragDelta += RightCornerThumb_DragDelta;
                }
            }
        }

        internal Border PopupBorder { get; set; }

        /// <summary>
        /// Gets or sets the grid control.
        /// </summary>
        /// <value>The grid control.</value>
        internal PivotGridControl GridControl { get; set; }
        /// <summary>
        /// Wires the apply button event.
        /// </summary>
        private void WireButtonApplyEvent()
        {
            this.OkButton.Click += new RoutedEventHandler(btnOk_Click);
        }

        /// <summary>
        /// Wires the button cancel event.
        /// </summary>
        private void WireButtonCancelEvent()
        {
            this.CancelButton.Click += new RoutedEventHandler(CancelButton_Click);
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CanExecute = false;
            this.IsOpen = false;
        }

        void RightCornerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.MinHeight <= (this.Height + e.VerticalChange))
            {
                this.Height += e.VerticalChange;
            }
            if (this.MinWidth <= (this.Width + e.HorizontalChange))
            {
                this.Width += e.HorizontalChange;
            }
        }

        internal List<FilterChoice> FilterListBoxItems { get; set; }
    }
}
