#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.DLS
{
    internal class SDTListContent
    {
        # region Fields
        private string m_lastValue;
        private List<ListItem> m_listItems = new List<ListItem>();
        # endregion

        #region Properties
        /// <summary>
        /// Gets the last value
        /// </summary>
        internal string LastValue
        {
            get
            {
                return m_lastValue;
            }
            set
            {
                m_lastValue = value;
            }
        }
        /// <summary>
        /// Gets the list items
        /// </summary>
        internal List<ListItem> ListItems
        {
            get
            {
                return m_listItems;
            }
        }

        # endregion
    }
    internal class SDTComboBox : SDTListContent
    {
        internal SDTComboBox()
        {
            //Empty constructor
        }
    }
    internal class SDTDropDownList : SDTListContent
    {
        internal SDTDropDownList()
        {
            //Empty constructor
        }
    }
    internal class ListItem
    {
        # region Fields
        private string m_displayText;
        private string m_value;
        # endregion

        # region Properties
        internal string DisplayText
        {
            get
            {
                return m_displayText;
            }
            set
            {
                m_displayText = value;
            }
        }
        internal string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }
        # endregion
    }
}
