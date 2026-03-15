#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.ObjectModel;
    using System.Collections;
    using Syncfusion.Linq;

    /// <summary>
    /// GroupEntry contains the list of groups for each sub-groups populated for the <see cref="TopLevelGroup"/> class.
    /// </summary>
    public class GroupEntry : NodeEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupEntry"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        public GroupEntry(GroupEntry parent, int level)
            : base(parent, level)
        {
            this.Groups = new List<Group>();
            this.IsGroups = true;
        }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public List<Group> Groups
        {
            get;
            private set;
        }

        //public virtual int GetGroupsCount()
        //{
        //    return this.Groups.Count;
        //}

        /// <summary>
        /// returns an array of SummaryDetails for each group.
        /// </summary>
        /// <returns></returns>
        public object[] ToSummaryArray()
        {
            return this.Groups.Select(o => o.SummaryDetails).ToArray();
        }
    }
}
