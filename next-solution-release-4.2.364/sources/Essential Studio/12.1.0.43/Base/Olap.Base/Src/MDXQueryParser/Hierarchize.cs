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

namespace Syncfusion.Olap.MDXQueryParser
{
    public class Hierarchize : IKeyword
    {
        #region IKeyword Members        
        public string Name{ get; set;}
        public DrillDownLevel DrillDownLevel { get; set; }
        public DrillDownMember DrillDownMember { get; set; }
        #endregion        
        

        public Hierarchize()
        {
            this.Name = string.Empty;
            this.DrillDownLevel = new DrillDownLevel();
            this.DrillDownMember = new DrillDownMember();
        }

        public bool Validate()
        {
            if (this.Name == null)
            {
                return false;
            }
            else if (this.DrillDownMember.Validate())
            {
                return true;
            }
            else return this.DrillDownLevel.Validate();
        }

        public new Hierarchize Clone()
        {
            Hierarchize hierarchize = new Hierarchize();
            hierarchize.Name = this.Name;
            hierarchize.DrillDownMember = this.DrillDownMember;
            hierarchize.DrillDownLevel = this.DrillDownLevel;
            return hierarchize;
        }
    }
}
