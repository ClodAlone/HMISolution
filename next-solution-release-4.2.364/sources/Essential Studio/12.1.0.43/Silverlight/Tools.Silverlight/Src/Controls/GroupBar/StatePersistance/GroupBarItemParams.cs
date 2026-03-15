#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Used for serializing GroupBarItem object in isolated storage ( to save GroupBaritem object state ).
    /// </summary>
    public class GroupBarItemParams
    {
        #region private members
        private bool isExpanded = false;
        private bool isSelected = false;
        private int itemIndex;
        private string itemName;
        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.isExpanded = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.isSelected = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the item.
        /// </summary>
        /// <value>The index of the item.</value>
        public int ItemIndex
        {
            get
            {
                return this.itemIndex;
            }

            set
            {
                this.itemIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        /// <value>The name of the item.</value>
        public string ItemName
        {
            get
            {
                return this.itemName;
            }

            set
            {
                this.itemName = value;
            }
        }

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the GroupBarParams class.
        /// </summary>
        public GroupBarItemParams()
        {
        }

        /// <summary>
        /// Initializes new instance of the GroupBarParams class.
        /// </summary>
        /// <param name="groupBarItem">GroupBar item params.</param>
        public GroupBarItemParams(GroupBarItem groupBarItem)
        {
            this.ItemName = groupBarItem.Name;
            this.IsExpanded = groupBarItem.IsExpanded;
            this.IsSelected = groupBarItem.IsSelected;
            this.ItemIndex = groupBarItem._groupBar.Items.IndexOf(groupBarItem);
        }
        #endregion

    }
}
