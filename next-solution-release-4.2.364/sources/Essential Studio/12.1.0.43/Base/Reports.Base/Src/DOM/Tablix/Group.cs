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
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class Group
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public string DocumentMapLabel { get; set; }
        public GroupExpressions GroupExpressions { get; set; }
        public string DomainScope { get; set; }
        public PageBreak PageBreak { get; set; }
        public Filters Filters { get; set; }
        public string Parent { get; set; }
        public string DataElementName { get; set; }

        public object Clone()
        {
            Group group = new Group();
            group.Name = this.Name;
            group.DocumentMapLabel = this.DocumentMapLabel;
            group.GroupExpressions = new DOM.GroupExpressions();
            group.Parent = this.Parent;
            group.DataElementName = this.DataElementName;
            group.DomainScope = this.DomainScope;
            group.Filters = new Filters();

            if (this.GroupExpressions != null)
            {
                group.GroupExpressions.AddRange(this.GroupExpressions.Clone());
            }

            if (this.PageBreak != null)
            {
                group.PageBreak = (PageBreak)this.PageBreak.Clone();
            }

            if (this.Filters != null)
            {
                group.Filters.AddRange(this.Filters.Clone());
            }

            return group;
        }

        public bool ShouldSerializeFilters()
        {
            return Filters != null && Filters.Count > 0;
        }

        public void ResetFilters()
        {
            this.Filters = new Filters();
        }

        public bool ShouldSerializeGroupExpressions()
        {
            return GroupExpressions != null && GroupExpressions.Count > 0;
        }

        public void ResetGroupExpressions()
        {
            this.GroupExpressions = new GroupExpressions();
        }
    }
}
