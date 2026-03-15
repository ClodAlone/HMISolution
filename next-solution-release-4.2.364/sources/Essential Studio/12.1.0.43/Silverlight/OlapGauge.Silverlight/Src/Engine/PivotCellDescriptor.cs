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

namespace Syncfusion.OlapSilverlight.Base.Engine
{

    public class Member
    {
        public Member()
        {
            this.KpiName = string.Empty;
        }

        public string UniqueName { get; set; }
        public string LevelUniqueName { get; set; }
        public string Caption { get; set; }
        public string KpiName { get; set; }
    }
    public class PivotCellDescriptor
    {
        public PivotCellDescriptor()
        {
            this.Range = new GridRangeInfo();
            this.CellExTypes = new List<string>();
        }

        public int CellIndex { get; set; }

        public bool HasChildrens { get; set; }
        public long Level { get; set; }
        public string CellValue { get; set; }
        public string Value { get; set; }
        public string CellCaption { get; set; }

        public string KpiGraphicsStyle { get; set; }
        public string ClassName { get; set; }
        public string UniqueName { get; set; }

        public KpiTypeEnum KpiType { get; set; }
        public ExpandableState ExpandableState { get; set; }
        public PivotCellDescriptorType CellType { get; set; }

        public PivotCellDescriptor SpanCell { get; set; }
        public GridRangeInfo Range { get; set; }
        public List<string> CellExTypes { get; set; }
        public Member MemberObjInfo { get; set; }
    }
}
