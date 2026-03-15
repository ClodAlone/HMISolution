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
using System.Collections;
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Manager;

namespace Syncfusion.Olap.MDXQueryParser
{    
    public class Hierarchies : IKeyword, ICloneable<Hierarchies>
    {
        public Hierarchies()
        {
            this.Members = new HierarchizeCollection();
        }

        #region Public Properties
        public HierarchizeCollection Members { get; set; }
        #endregion

        #region Public Methods
        public void Add(Hierarchize hierarchize)
        {
            this.Members.Add(hierarchize);
        }

        public new Hierarchies Clone()
        {
            Hierarchies hierarchies = new Hierarchies();
            hierarchies.Members = this.Members.Clone();
            return hierarchies;
        }
        #endregion

        #region IKeyword Members

        public string Name { get; set; }

        public bool Validate()
        {
            foreach (Hierarchize hierarchize in this.Members)
            {
                if (hierarchize.Validate())
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
