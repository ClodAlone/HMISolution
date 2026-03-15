#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Represents a single item in PivotTable field.
    /// </summary>
  public  class PivotItemOptions
    {
        #region members
        /// <summary>
        /// Specifies a boolean value that indicates whether the approximate number of child items
        ///for this item is greater than zero.
        /// </summary>
        private bool m_bHasChildItems;
        /// <summary>
        /// Specifies a boolean value that indicates whether this item has been expanded in the
        ///PivotTable view.
        /// </summary>
        private bool m_bIsExpaned;
        /// <summary>
        /// Specifies a boolean value that indicates whether attribute hierarchies nested next to
        ///each other on a PivotTable row or column will offer drilling "across" each other or not
        /// </summary>
        private bool m_bDrillAcross;
        /// <summary>
        /// Specifies a boolean value that indicates whether this item is a calculated member
        /// </summary>
        private bool m_bIsCalculatedItem;
        /// <summary>
        /// Specifies a boolean value that indicates whether the item is hidden.
        /// </summary>
        private bool m_bIsHidden;
        /// <summary>
        /// Specifies a boolean value that indicate whether the item has a missing value.
        /// </summary>
        private bool m_bIsMissing;
        /// <summary>
        /// Specifies the user caption of the item.
        /// </summary>
        private string m_userCaption;
        /// <summary>
        /// Specifies a boolean value that indicates whether the item has a character value.
        /// </summary>
        private bool m_bIsChar;
        /// <summary>
        /// Specifies a boolean value that indicates whether the details are hidden for this item.
        /// </summary>
        private bool m_bIsHiddenDetails;
        /// <summary>
        /// Specifies the type of the item. Value of 'default' indicates a grand total as the last row
        ///item value
        /// </summary>
        private PivotItemType m_itemType;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies a boolean value that indicates whether the approximate number of child items
        ///for this item is greater than zero.
        /// </summary>
        public bool HasChildItems
        {
            get
            {
                return m_bHasChildItems;
            }
            set
            {
                m_bHasChildItems = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether this item has been expanded in the
        ///PivotTable view.
        /// </summary>
        public bool IsExpaned
        {
            get
            {
                return m_bIsExpaned;
            }
            set
            {
                m_bIsExpaned = value;
            }
        }

        /// <summary>
        /// Specifies a boolean value that indicates whether attribute hierarchies nested next to
        ///each other on a PivotTable row or column will offer drilling "across" each other or not
        /// </summary>
        public bool DrillAcross
        {
            get
            {
                return m_bDrillAcross;
            }
            set
            {
                m_bDrillAcross = value;
            }
        }

        /// <summary>
        /// Specifies a boolean value that indicates whether this item is a calculated member
        /// </summary>
        public bool IsCalculatedItem
        {
            get
            {
                return m_bIsCalculatedItem;
            }
            set
            {
                m_bIsCalculatedItem = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the item is hidden.
        /// </summary>
        public bool IsHidden
        {
            get
            {
                return m_bIsHidden;
            }
            set
            {
                m_bIsHidden = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicate whether the item has a missing value.
        /// </summary>
        public bool IsMissing
        {
            get
            {
                return m_bIsMissing;
            }
            set
            {
                m_bIsMissing = value;
            }
        }


        /// <summary>
        /// Specifies the user caption of the item.
        /// </summary>
        public string UserCaption
        {
            get
            {
                return m_userCaption;
            }
            set
            {
                m_userCaption = value;
            }
        }

        /// <summary>
        /// Specifies a boolean value that indicates whether the item has a character value.
        /// </summary>
        public bool IsChar
        {
            get
            {
                return m_bIsChar;
            }
            set
            {
                m_bIsChar = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the details are hidden for this item.
        /// </summary>
        public bool IsHiddenDetails
        {
            get
            {
                return m_bIsHiddenDetails;
            }
            set
            {
                m_bIsHiddenDetails = value;
            }
        }
        /// <summary>
        /// Specifies the type of the item. Value of 'default' indicates a grand total as the last row
        ///item value
        /// </summary>
        public PivotItemType ItemType
        {
            get
            {
                return m_itemType;
            }
            set
            {
                m_itemType = value;
            }
        }

        #endregion

        #region Initialize
        public PivotItemOptions()
        {
            
        }
        #endregion
    }
}
