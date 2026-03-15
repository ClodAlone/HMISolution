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
    public class HierarchizeCollection : CollectionBase, ICloneable<HierarchizeCollection>
    {
        public HierarchizeCollection()
        {
        }

        public Hierarchize this[int index]
        {
            get
            {
                return (Hierarchize)base.List[index];
            }

            set
            {
                base.List[index] = value;
            }
        }

        public int Add(Hierarchize hierarchize)
        {
            return base.List.Add(hierarchize);
        }

        public void Insert(int index, Hierarchize hierarchize)
        {
            base.List.Insert(index, hierarchize);
        }

        public void Remove(Hierarchize hierarchize)
        {
            base.List.Remove(hierarchize);
        }

        public HierarchizeCollection Clone()
        {
            HierarchizeCollection hierarchizeCollection = new HierarchizeCollection();
            foreach (Hierarchize measureElement in hierarchizeCollection)
            {
                hierarchizeCollection.Add(measureElement.Clone());
            }

            return hierarchizeCollection;
        }
    }
}
