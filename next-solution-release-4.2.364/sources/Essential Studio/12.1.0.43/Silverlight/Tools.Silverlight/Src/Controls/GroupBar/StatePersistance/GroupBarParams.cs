#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Used for serializing GroupBar object in isolated storage ( to save GroupBar object state ).
    /// </summary>
    [XmlRootAttribute(ElementName = "GroupBarParams", IsNullable = false)]
    public class GroupBarParams
    {
        #region Private members
        /// <summary>
        /// GroupBar items.
        /// </summary>
        private List<GroupBarItemParams> barItems;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets GroupBar items.
        /// </summary>
        /// <value>
        /// Type: <see cref="GroupBarItemParams"/>
        /// </value>
        /// <seealso cref="GroupBarItemParams"/>
        public List<GroupBarItemParams> GroupBarItems
        {
            get
            {
                return this.barItems;
            }

            set
            {
                this.barItems = value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes new instance of the GroupBarParams class.
        /// </summary>
        public GroupBarParams()
        {
        }

        /// <summary>
        /// Initializes new instance of the GroupBarParams class.
        /// </summary>
        /// <param name="groupBar">GroupBar object to serialize.</param>
        public GroupBarParams(GroupBar groupBar)
        {
            this.barItems = new List<GroupBarItemParams>();
            foreach (GroupBarItem item in groupBar.Items)
            {
                GroupBarItemParams itemParam = new GroupBarItemParams(item);
                this.barItems.Add(itemParam);
            }
        }

        #endregion
    }
}
