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
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
    public class TablixMembers : List<TablixMember>
    {
    }

    public class TablixMember
    {
        public Group Group { get; set; }
        public SortExpressions SortExpressions { get; set; }
        public TablixHeader TablixHeader { get; set; }
        public TablixMembers TablixMembers { get; set; }
        public CustomProperties CustomProperties { get; set; }
        public bool FixedData { get; set; }
        public Visibility Visibility { get; set; }
        public bool HideIfNoRows { get; set; }
        public KeepWithGroup KeepWithGroup { get; set; }
        public bool RepeatOnNewPage { get; set; }
        public string DataElementName { get; set; }

        [DefaultValue(DataElementOutputs.Auto)]
        public DataElementOutputs DataElementOutput { get; set; }
        public bool KeepTogether { get; set; }

        public object Clone()
        {
            TablixMember tablixMember = new TablixMember();
            tablixMember.HideIfNoRows = this.HideIfNoRows;
            tablixMember.KeepWithGroup = this.KeepWithGroup;
            tablixMember.RepeatOnNewPage = this.RepeatOnNewPage;
            tablixMember.DataElementName = this.DataElementName;
            tablixMember.DataElementOutput = this.DataElementOutput;
            tablixMember.KeepTogether = this.KeepTogether;
            tablixMember.FixedData = this.FixedData;
            tablixMember.SortExpressions = new DOM.SortExpressions();
            tablixMember.TablixMembers = new DOM.TablixMembers();
            tablixMember.CustomProperties = new DOM.CustomProperties();

            if (this.Group != null)
            {
                tablixMember.Group = (Group)this.Group.Clone();
            }

            if (this.SortExpressions != null)
            {
                tablixMember.SortExpressions.AddRange(this.SortExpressions.Clone());
            }

            if (this.TablixHeader != null)
            {
                tablixMember.TablixHeader = (TablixHeader)this.TablixHeader.Clone();
            }

            if (this.TablixMembers != null)
            {
                tablixMember.TablixMembers.AddRange(this.TablixMembers.Clone());
            }

            if (this.CustomProperties != null)
            {
                tablixMember.CustomProperties.AddRange(this.CustomProperties.Clone());
            }

            if (this.Visibility != null)
            {
                tablixMember.Visibility = (Visibility)this.Visibility.Clone();
            }
            return tablixMember;
        }

        public bool ShouldSerializeSortExpressions()
        {
            return SortExpressions != null && SortExpressions.Count > 0;
        }

        public void ResetSortExpressions()
        {
            this.SortExpressions = new SortExpressions();
        }

        public bool ShouldSerializeCustomProperties()
        {
            return CustomProperties != null && CustomProperties.Count > 0;
        }

        public void ResetCustomProperties()
        {
            this.CustomProperties = new CustomProperties();
        }

        public bool ShouldSerializeTablixMembers()
        {
            return TablixMembers != null && TablixMembers.Count > 0;
        }

        public void ResetCustomTablixMembers()
        {
            this.TablixMembers = new TablixMembers();
        }

        public bool ShouldSerializeHideIfNoRows()
       {
            return this.HideIfNoRows != false;
        }

        public void ResetHideIfNoRows()
        {
            this.HideIfNoRows = false;
        }

        public bool ShouldSerializeKeepWithGroup()
        {
            return this.KeepWithGroup != KeepWithGroup.None;
        }

        public void ResetKeepWithGroup()
        {
            this.KeepWithGroup = KeepWithGroup.None;
        }

        public bool ShouldSerializeFixedData()
        {
            return this.FixedData != false;
        }

        public void ResetFixedData()
        {
            this.FixedData = false;
        }

        public bool ShouldSerializeRepeatOnNewPage()
        {
            return this.RepeatOnNewPage != false;
        }

        public void ResetRepeatOnNewPage()
        {
            this.RepeatOnNewPage = false;
        }

        public bool ShouldSerializeKeepTogether()
        {
            return this.KeepTogether != false;
        }

        public void ResetKeepTogether()
        {
            this.KeepTogether = false;
        }
   }
}
