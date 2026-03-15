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
    public class Tablix : DataRegion
    {
        
        public TablixCorner TablixCorner { get; set; }
        public TablixBody TablixBody { get; set; }
        public TablixColumnHierarchy TablixColumnHierarchy { get; set; }
        public TablixRowHierarchy TablixRowHierarchy { get; set; }

        [DefaultValue(Direction.Default)]
        public Direction LayoutDirection { get; set; }

        [DefaultValue(0)]
        public int GroupsBeforeRowHeader { get; set; }

        public bool RepeatColumnHeaders { get; set; }
        public bool RepeatRowHeaders { get; set; }
        public bool FixedColumnHeaders { get; set; }
        public bool FixedRowHeaders { get; set; }
        public bool KeepTogether { get; set; }

        public object Clone()
        {
            Tablix tablix = new Tablix();
            tablix.Name = this.Name;
            tablix.Left = this.Left;
            tablix.Top = this.Top;
            tablix.LayoutDirection = this.LayoutDirection;
            tablix.GroupsBeforeRowHeader = this.GroupsBeforeRowHeader;
            tablix.RepeatColumnHeaders = this.RepeatColumnHeaders;
            tablix.RepeatRowHeaders = this.RepeatRowHeaders;
            tablix.FixedColumnHeaders = this.FixedColumnHeaders;
            tablix.FixedRowHeaders = this.FixedRowHeaders;
            tablix.KeepTogether = this.KeepTogether;
            tablix.DataElementName = this.DataElementName;
            tablix.DataSetName = this.DataSetName;
            tablix.NoRowsMessage = this.NoRowsMessage;
            tablix.PageName = this.PageName;
            tablix.TablixBody = (TablixBody)this.TablixBody.Clone();
            tablix.Width = (Size)this.Width.Clone();
            tablix.Height = (Size)this.Height.Clone();
            tablix.TablixColumnHierarchy = (TablixColumnHierarchy)this.TablixColumnHierarchy.Clone();
            tablix.TablixRowHierarchy = (TablixRowHierarchy)this.TablixRowHierarchy.Clone();
            tablix.SortExpressions = new DOM.SortExpressions();
            tablix.Filters = new DOM.Filters();

            if (this.TablixCorner != null)
            {
                tablix.TablixCorner = (TablixCorner)this.TablixCorner.Clone();
            }

            if (this.PageBreak != null)
            {
                tablix.PageBreak = (PageBreak)this.PageBreak.Clone();
            }

            if (this.SortExpressions != null)
            {
                tablix.SortExpressions.AddRange(this.SortExpressions.Clone());
            }

            if (this.Filters != null)
            {
                tablix.Filters.AddRange(this.Filters.Clone());
            }


            return tablix;
        }

        public bool ShouldSerializeRepeatColumnHeaders()
        {
            return this.RepeatColumnHeaders != false;
        }

        public void ResetRepeatColumnHeaders()
        {
            this.RepeatColumnHeaders = false;
        }

        public bool ShouldSerializeRepeatRowHeaders()
        {
            return this.RepeatRowHeaders != false;
        }

        public void ResetRepeatRowHeaders()
        {
            this.RepeatRowHeaders = false;
        }

        public bool ShouldSerializeFixedColumnHeaders()
        {
            return this.FixedColumnHeaders != false;
        }

        public void ResetFixedColumnHeaders()
        {
            this.FixedColumnHeaders = false;
        }

        public bool ShouldSerializeFixedRowHeaders()
        {
            return this.FixedRowHeaders != false;
        }

        public void ResetFixedRowHeaders()
        {
            this.FixedRowHeaders = false;
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
