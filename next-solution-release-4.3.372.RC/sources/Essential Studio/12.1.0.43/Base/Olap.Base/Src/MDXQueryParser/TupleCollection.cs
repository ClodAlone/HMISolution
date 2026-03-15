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

namespace Syncfusion.Olap.MDXQueryParser
{
    public class TupleCollection : CollectionBase
    {
        public TupleCollection()
        {
        }

        public Tuple this[int index]
        {
            get
            {
                return (Tuple)base.List[index];
            }

            set
            {
                base.List[index] = value;
            }
        }

        public int Add(Tuple tuple)
        {
            return base.List.Add(tuple);
        }

        public void Insert(int index, Tuple tuple)
        {
            base.List.Insert(index, tuple);
        }

        public void Remove(Tuple tuple)
        {
            base.List.Remove(tuple);
        }
    }
}
