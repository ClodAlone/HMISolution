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

namespace Syncfusion.RDL.DOM
{
    public class TablixHierarchy
    {
        public TablixMembers TablixMembers { get; set; }

    }

    public class TablixRowHierarchy : TablixHierarchy
    {
        public object Clone()
        {
            TablixRowHierarchy tablixHierarchy = new TablixRowHierarchy();
            tablixHierarchy.TablixMembers = new TablixMembers();
            tablixHierarchy.TablixMembers.AddRange(this.TablixMembers.Clone());
            return tablixHierarchy;
        }
    }

    public class TablixColumnHierarchy: TablixHierarchy
    {
        public object Clone()
        {
            TablixColumnHierarchy tablixHierarchy = new TablixColumnHierarchy();
            tablixHierarchy.TablixMembers = new TablixMembers();
            tablixHierarchy.TablixMembers.AddRange(this.TablixMembers.Clone());
            return tablixHierarchy;
        }
    }
}
