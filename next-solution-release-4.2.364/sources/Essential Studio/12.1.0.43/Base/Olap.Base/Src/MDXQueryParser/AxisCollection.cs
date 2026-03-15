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
    public class AxisCollection : CollectionBase
    {
        public AxisCollection()
        {
        }

        public Axis this[int index]
        {
            get
            {
                return (Axis)base.List[index];
            }

            set
            {
                base.List[index] = value;
            }
        }

        public int Add(Axis axis)
        {
            return base.List.Add(axis);
        }

        public void Insert(int index, Axis axis)
        {
            base.List.Insert(index, axis);
        }

        public void Remove(Axis axis)
        {
            base.List.Remove(axis);
        }
    }
}
