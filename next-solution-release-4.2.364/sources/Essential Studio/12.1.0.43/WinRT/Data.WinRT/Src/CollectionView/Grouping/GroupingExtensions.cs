#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Linq.Expressions;
    
    /// <summary>
    /// Extensions for <see cref="Group"/> class.
    /// </summary>
    public static class GroupingExtensions
    {
        /// <summary>
        /// Expands all groups at the specified level.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="level">The level.</param>
        public static void ExpandAllAtLevel(this Group group, int level)
        {
            foreach (var innergroup in group.Groups)
            {
                if (innergroup.Level == level)
                {
                    innergroup.IsExpanded = true;
                    if (innergroup.IsBottomLevel)
                    {
                        var topLevelGroup = innergroup.GetTopLevelGroup();
                        topLevelGroup.UpdateSummaries(innergroup);
                    }
                }
                else if (!innergroup.IsBottomLevel)
                {
                    innergroup.ExpandAllAtLevel(level);
                }
            }
        }

        /// <summary>
        /// Collapses all groups at the specified level.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="level">The level.</param>
        public static void CollapseAllAtLevel(this Group group, int level)
        {
            foreach (var innergroup in group.Groups)
            {
                if (innergroup.Level == level)
                {
                    innergroup.IsExpanded = false;
                }
                else
                {
                    innergroup.ExpandAllAtLevel(level);
                }
            }
        }

        /// <summary>
        /// Expands all groups.
        /// </summary>
        /// <param name="group">The group.</param>
        public static void ExpandAll(this Group group)
        {
            if (group != null)
            {
                group.Groups.ExpandCollapseAll(true);
                var topLevelGroup = group.GetTopLevelGroup();
                topLevelGroup.ResetCache = true;
            }
        }

        /// <summary>
        /// Collapses all groups.
        /// </summary>
        /// <param name="group">The group.</param>
        public static void CollapseAll(this Group group)
        {
            if (group != null)
            {
                group.Groups.ExpandCollapseAll(false);
                var topLevelGroup = group.GetTopLevelGroup();
                topLevelGroup.ResetCache = true;
            }
        }

        /// <summary>
        /// Gets the top level group.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public static TopLevelGroup GetTopLevelGroup(this NodeEntry entry)
        {
            var group = entry;
            while (group.Parent != null)
            {
                group = group.Parent;
            }

            return (group as TopLevelGroup);
        }

        static void ExpandCollapseAll(this List<Group> groups, bool value)
        {
            foreach (var group in groups)
            {
                if (group.IsGroups)
                {
                    var groupEntry = group as Group;
                    
                    if(groupEntry.IsExpanded != value)
                        groupEntry.IsExpanded = value;
                    
                    if (!groupEntry.IsBottomLevel)
                    {
                        groupEntry.Groups.ExpandCollapseAll(value);
                    }
                }
            }
        }

        public static void SetDirty(this List<Group> groups)
        {
            if (groups == null)
                return;

            foreach (var group in groups)
            {   
                group.SetDirty();

                if (!group.IsBottomLevel)
                {
                    group.Groups.SetDirty();
                }
            }
        }
    }
}
