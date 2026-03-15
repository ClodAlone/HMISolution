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

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    public class HiddenGroup
    {
        public HiddenGroup()
        {

        }
        public HiddenGroup(int from, int to, int level, string groupName)
            : this(from, to, level, groupName,String.Empty)
        {

        }


        public HiddenGroup(int from, int to, int level, string groupName,string totalHeader)
        {
            this.From = from;
            this.To = to;
            this.Level = level;
            this.GroupName = groupName;
            this.ItemTotalHeader=totalHeader;
        }

        public int From { get; set; }

        public int To { get; set; }

        public int Level { get; set; }

        public string GroupName { get; set; }

        public string ItemTotalHeader { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", this.From, this.To);
        }

        public HiddenGroup Clone(HiddenGroup ParentGroup)
        {
            HiddenGroup m_HiddenGroup = new HiddenGroup();
            m_HiddenGroup.From = ParentGroup.From;
            m_HiddenGroup.To = ParentGroup.To;
            m_HiddenGroup.GroupName = ParentGroup.GroupName;
            m_HiddenGroup.Level = ParentGroup.Level;
            return m_HiddenGroup;
        }
    }

    public static class ExtensionClass
    {
        /// <summary>
        /// Determines whether the specified item is in the collection
        /// </summary>
        /// <param name="t">The Collection.</param>
        /// <param name="hiddenGroup">The hidden group.</param>
        public static bool Has(this List<HiddenGroup> t,HiddenGroup hiddenGroup)
        {
            bool isPresent = false;

            foreach (var item in t)
            {
                if (item.From == hiddenGroup.From && item.To == hiddenGroup.To &&
             item.GroupName == hiddenGroup.GroupName && item.Level == hiddenGroup.Level)
                {
                    isPresent = true;
                    break;
                }
            }

            return isPresent;
        }
    }
}
